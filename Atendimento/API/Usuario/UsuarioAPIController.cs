
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using WebDenunciaSSP.Atendimento.Models;
using WebDenunciaSSP.CommonLibs.Models;
using WebDenunciaSSP.Entidades;
using WebDenunciaSSP.Entidades.Context;

namespace WebDenunciaSSP.Atendimento.API
{
    public class UsuarioAPIController : ApiController
    {
        private readonly WDContext _dbContext;
        private readonly IAuthenticationManager _authenticationManager;
        private SHA256 crypt;
        private readonly IOwinContext _owinContext;

        public UsuarioAPIController(WDContext context, IAuthenticationManager authenticationManager, IOwinContext owinContext)
        {
            _dbContext = context;
            _authenticationManager = authenticationManager;
            _owinContext = owinContext;
        }

        public IHttpActionResult listaUsuarios()
        {
            List<Usuario> lista = _dbContext.Usuario.Where(x => x.FLAG_ATIVO).ToList();

            return Ok(lista);
        }

        [HttpGet]
        public IHttpActionResult listarOrgaosPorTipo(int idTipo)
        {
            try
            {
                List<OrgaoViewModel> lista = new List<OrgaoViewModel>();
                List<Orgao> orgaos = new List<Orgao>();
                orgaos = _dbContext.Orgao.Where(o => o.ID_TIPO_ORGAO == idTipo).ToList();

                short low = orgaos.Min(x => x.ID_HIERARCHY.GetLevel());
                List<short> grp = orgaos.GroupBy(x => x.ID_HIERARCHY.GetLevel()).Select(x => x.Key).OrderBy(x => x).ToList();

                List<Orgao> pais = orgaos.Where(o => o.ID_HIERARCHY.GetLevel() == low).ToList();

                foreach (Orgao org in pais)
                {
                    short next = grp.FirstOrDefault(x => x > low);
                    List<Orgao> filhos = orgaos.Where(f => f.ID_HIERARCHY.IsDescendantOf(org.ID_HIERARCHY) && f.ID_HIERARCHY.GetLevel() == next).ToList();

                    lista.Add(new OrgaoViewModel()
                    {
                        idOrgao = org.ID_ORGAO,
                        nomeOrgao = org.NOME_ORGAO,
                        children = carregarFilhosOrgao(next, filhos, orgaos, grp),
                    });
                }

                return Ok(lista);            
            } catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return BadRequest(ModelState);
            }
        }

        private List<OrgaoViewModel> carregarFilhosOrgao(short level, List<Orgao> orgFilhos, List<Orgao> completo, List<short> niveis)
        {
            try
            {
                List<OrgaoViewModel> lista = new List<OrgaoViewModel>();

                foreach (Orgao filho in orgFilhos)
                {
                    short next = niveis.FirstOrDefault(x => x > level);
                    List<Orgao> filhos = completo.Where(f => f.ID_HIERARCHY.IsDescendantOf(filho.ID_HIERARCHY) && f.ID_HIERARCHY.GetLevel() == next).ToList();

                    lista.Add(new OrgaoViewModel()
                    {
                        idOrgao = filho.ID_ORGAO,
                        nomeOrgao = filho.NOME_ORGAO,
                        children = carregarFilhosOrgao(next, filhos, completo, niveis),
                    });
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public IHttpActionResult Logar(RegistroUsuarioViewModel model)
        {
            //Criando o hash da primeira senha (CPF) do novo usuário
            crypt = SHA256Managed.Create();

            Usuario user = _dbContext.Usuario.FirstOrDefault(c => c.LOGIN == model.usuarioLogin && c.FLAG_ATIVO );

            if (user != null) {
                string pass = model.Senha;

                byte[] passbytes = Encoding.UTF8.GetBytes(user.SALT + pass);
                byte[] hash = crypt.ComputeHash(passbytes);

                if (Convert.ToBase64String(hash) == user.SENHA )
                {
                    UsuarioLog ulog = new UsuarioLog();
                    ulog.ID_USUARIO = user.ID_USUARIO;
                    ulog.DATA_LOG = DateTime.Now;
                    ulog.DESCRICAO_LOG = string.Format("O atendente '{0}' realizou o login no sistema de atendimento do DEPA.", user.NOME_COMPLETO);
                    _dbContext.UsuarioLog.Add(ulog);
                    _dbContext.SaveChanges();

                    UsuarioModel userModel = new UsuarioModel()
                    {
                        ID_USUARIO = user.ID_USUARIO,
                        ID_GRUPO = user.ID_GRUPO,
                        ID_ORGAO = user.ID_ORGAO.HasValue ? user.ID_ORGAO.Value : (int?)null,
                        LOGIN = user.LOGIN,
                        NOME = user.NOME_COMPLETO,
                        EMAIL = user.EMAIL,
                        GRUPO = user.Grupo.DESCRICAO_GRUPO,
                        ORGAO = user.ID_ORGAO.HasValue ? _dbContext.Orgao.FirstOrDefault(o => o.ID_ORGAO == user.ID_ORGAO.Value).NOME_ORGAO : string.Empty,
                        FLAG_CIVIL = new List<int>() { 1, 2, 4, 6 }.Any(i => i == user.ID_GRUPO),
                        FLAG_MILITAR = new List<int>() { 3, 5, 7 }.Any(i => i == user.ID_GRUPO),
                        FLAG_ACESSO = user.PRIMEIRO_ACESSO
                    };

                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationType);
                    
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.ID_USUARIO.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.LOGIN));
                    identity.AddClaim(new Claim(ClaimTypes.Role, user.Grupo.DESCRICAO_GRUPO));

                    if (user.ID_ORGAO.HasValue)
                    {
                        identity.AddClaim(new Claim("Orgao", user.ID_ORGAO.Value.ToString()));
                    }
                    _authenticationManager.SignOut(CookieAuthenticationDefaults.AuthenticationType);
                    _authenticationManager.SignIn(identity);

                    return Ok(userModel);
                }else
                {
                    return BadRequest("Usuário ou senha incorretos");
                }
            }
            else
            {
                return BadRequest("Usuário ou senha incorretos");
            }
        }

        [HttpPost]
        public IHttpActionResult Logout()
        {
            int idUsuario = Convert.ToInt32(_authenticationManager.User.Identity.Name);
            Usuario user = _dbContext.Usuario.FirstOrDefault(x => x.ID_USUARIO == idUsuario);

            UsuarioLog ulog = new UsuarioLog();
            ulog.ID_USUARIO = user.ID_USUARIO;
            ulog.DATA_LOG = DateTime.Now;
            ulog.DESCRICAO_LOG = string.Format("O atendente '{0}' realizou o logout do sistema de atendimento do DEPA.", user.NOME_COMPLETO);
            _dbContext.UsuarioLog.Add(ulog);
            _dbContext.SaveChanges();

            _authenticationManager.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult ResetarSenhaUsuario(object modelusuario)
        {
            dynamic ususario = JsonConvert.DeserializeObject(modelusuario.ToString());

            string CPF = ususario.CPF;
            string ID = ususario.ID;

            int ID_ADMIN = Convert.ToInt32(ususario.ID_ADMIN);

            //Criando o SALT
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] number = new byte[32];
            rng.GetBytes(number);
            string pass_salt = Convert.ToBase64String(number);

            //Criando o hash da primeira senha (CPF) do novo usuário
            crypt = SHA256Managed.Create();

            string senha = "depa123";

            if (!String.IsNullOrWhiteSpace(CPF))
                senha = CPF;

            string pass = senha;
            byte[] passbytes = Encoding.UTF8.GetBytes(pass_salt + pass);
            byte[] hash = crypt.ComputeHash(passbytes);
            int id = Convert.ToInt32(ID);

            if (id > 0)
            {
                Usuario user = _dbContext.Usuario.FirstOrDefault(c => c.ID_USUARIO == id && c.FLAG_ATIVO);
                user.SALT = pass_salt;
                user.SENHA = Convert.ToBase64String(hash);

                UsuarioLog ulog = new UsuarioLog();
                ulog.ID_USUARIO = ID_ADMIN;
                ulog.ID_USUARIO_ALTERADO = id;
                ulog.DATA_LOG = DateTime.Now;
                ulog.DESCRICAO_LOG = string.Format("O usuário '{0}' teve a sua senha resetada pelo administrador '{1}'.", user.NOME_COMPLETO, ususario.NOME_ADMIN);
                _dbContext.UsuarioLog.Add(ulog);

                _dbContext.SaveChanges();

                return Ok("Senha Alterada com Sucesso");
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        public IHttpActionResult InativarUsuario(object modelusuario)
        {
            dynamic ususario = JsonConvert.DeserializeObject(modelusuario.ToString());

            string ID = ususario.ID;
            int ID_ADMIN = Convert.ToInt32(ususario.ID_ADMIN);
            int id = Convert.ToInt32(ID);

            if (id > 0)
            {
                Usuario user = _dbContext.Usuario.FirstOrDefault(c => c.ID_USUARIO == id);
                user.FLAG_ATIVO = false;

                UsuarioLog ulog = new UsuarioLog();
                ulog.ID_USUARIO = ID_ADMIN;
                ulog.ID_USUARIO_ALTERADO = id;
                ulog.DATA_LOG = DateTime.Now;
                ulog.DESCRICAO_LOG = string.Format("O usuário '{0}' foi inativado pelo administrador '{1}'.", user.NOME_COMPLETO, ususario.NOME_ADMIN);
                _dbContext.UsuarioLog.Add(ulog);

                _dbContext.SaveChanges();

                return Ok("Usuário Inativo");
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        public IHttpActionResult ResetarSenha(RegistroUsuarioViewModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "Error");
                return BadRequest(ModelState);
            }

            if (model.Senha != "")
            {
                //Criando o SALT
                RandomNumberGenerator rng = RandomNumberGenerator.Create();
                byte[] number = new byte[32];
                rng.GetBytes(number);
                string pass_salt = Convert.ToBase64String(number);

                //Criando o hash da primeira senha (CPF) do novo usuário
                crypt = SHA256Managed.Create();

                string senha = model.NovaSenha;
                string pass = senha;

                byte[] passbytes = Encoding.UTF8.GetBytes(pass_salt + pass);
                byte[] hash = crypt.ComputeHash(passbytes);
                int id = Convert.ToInt16(_authenticationManager.User.Identity.Name);

                if (id > 0)
                {

                    Usuario user = _dbContext.Usuario.FirstOrDefault(c => c.ID_USUARIO == id);

                    user.SENHA = Convert.ToBase64String(hash);
                    user.SALT = pass_salt;

                    UsuarioLog ulog = new UsuarioLog();
                    ulog.ID_USUARIO = id;
                    ulog.DATA_LOG = DateTime.Now;
                    ulog.DESCRICAO_LOG = string.Format("O usuário '{0}' alterou a sua senha.", user.NOME_COMPLETO);
                    _dbContext.UsuarioLog.Add(ulog);

                    _dbContext.SaveChanges();
                    return Ok();
                }
            }

            return Ok("Senha Alterada com Sucesso");
        }

        [HttpPost]
        public IHttpActionResult CreateUsuario(RegistroUsuarioViewModel model)
        {
            // Recupera o ID do usuário que está cadastrando ou alterando um usuário
            int idAdmin = Convert.ToInt32(_authenticationManager.User.Identity.Name);
            Usuario admin = _dbContext.Usuario.FirstOrDefault(x => x.ID_USUARIO == idAdmin && x.FLAG_ATIVO);

            //Criando o SALT
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] number = new byte[32];
            rng.GetBytes(number);
            string pass_salt = Convert.ToBase64String(number);

            //Criando o hash da primeira senha (CPF) do novo usuário
            crypt = SHA256Managed.Create();

            string senha = "depa123";
            if (!String.IsNullOrWhiteSpace(model.usuarioCPF))
                senha = model.usuarioCPF;

            string pass = senha;
            byte[] passbytes = Encoding.UTF8.GetBytes(pass_salt + pass);
            byte[] hash = crypt.ComputeHash(passbytes);

            //Passando as informações de senha e salt para o usuário
            // novo.SENHA = Convert.ToBase64String(hash);
            //novo.SALT = pass_salt;

            if (model == null)
            {
                ModelState.AddModelError("", "Error");
                return BadRequest(ModelState);
            }

            if (!model.TipoUsuario)
            {
                ModelState.Remove("model.usuarioRG");
                ModelState.Remove("model.usuarioCPF");
                ModelState.Remove("model.usuarioEmail");
                ModelState.Remove("model.Senha");
                ModelState.Remove("model.NovaSenha");
                ModelState.Remove("model.ConfirmaNovaSenha");
            }
            else
            {
                ModelState.Remove("model.Senha");
                ModelState.Remove("model.NovaSenha");
                ModelState.Remove("model.ConfirmaNovaSenha");
            }

            if (!ModelState.IsValid)
            {
               // return BadRequest(ModelState);


                return BadRequest("Preencha os campos obrigatorios marcados com  *  ");
            }

            int qtd = _dbContext.Usuario.Count(x => x.LOGIN == model.usuarioLogin);

            int cpf = _dbContext.Usuario.Count(x => x.CPF == model.usuarioCPF);

            if (cpf > 0 && model.usuarioID == 0)
            {
                return BadRequest("CPF já cadastrado");
            }


            if (qtd == 0 && model.usuarioID == 0)
            {
                _dbContext.Usuario.Add(new Usuario()
                {
                    ID_USUARIO = model.usuarioID,
                    LOGIN = model.usuarioLogin,
                    NOME_COMPLETO = model.usuarioNome,
                    EMAIL = model.usuarioEmail,
                    CPF = model.usuarioCPF,
                    RG = model.usuarioRG,
                    RG_UF = model.usuarioRG_UF,
                    ID_GRUPO = model.GrupoId,
                    ID_ORGAO = model.OrgaoId,
                    DATA_ALTERADO = DateTime.Now,
                    DATA_CADASTRO = DateTime.Now,
                    FLAG_ATIVO = true,
                    PRIMEIRO_ACESSO = true,
                    SALT = pass_salt,
                    SENHA = Convert.ToBase64String(hash),
                });

                UsuarioLog ulog = new UsuarioLog();
                ulog.ID_USUARIO = idAdmin;
                ulog.DATA_LOG = DateTime.Now;
                ulog.DESCRICAO_LOG = string.Format("O administrador '{0}' realizou o cadastro de um novo usuário.", admin.NOME_COMPLETO);
                _dbContext.UsuarioLog.Add(ulog);

                _dbContext.SaveChanges();

                return Ok("Usuário cadastrado com sucesso!");
            }
            else if(model.usuarioID ==0)
            {               
                return BadRequest("Login já cadastrado");
            }

            if (model.usuarioID > 0)
            {

                Usuario user = _dbContext.Usuario.FirstOrDefault(c => c.ID_USUARIO == model.usuarioID);

                user.NOME_COMPLETO = model.usuarioNome;
                user.EMAIL = model.usuarioEmail;
                user.CPF = model.usuarioCPF;
                user.RG = model.usuarioRG;
                user.RG_UF = model.usuarioRG_UF;
                user.ID_GRUPO = model.GrupoId;
                user.ID_ORGAO = model.OrgaoId;
                user.DATA_ALTERADO = DateTime.Now;

                UsuarioLog ulog = new UsuarioLog();
                ulog.ID_USUARIO = idAdmin;
                ulog.DATA_LOG = DateTime.Now;
                ulog.DESCRICAO_LOG = string.Format("O administrador '{0}' alterou o cadastro do usuário '{1}'.", admin.NOME_COMPLETO, user.NOME_COMPLETO);
                _dbContext.UsuarioLog.Add(ulog);

                _dbContext.SaveChanges();

                return Ok("Usuário alterado com sucesso!");
            }            

            return BadRequest(ModelState);
        }

        [HttpPost]
        public IHttpActionResult CriarUsuario(RegistroUsuarioViewModel modelo)
        {
            ModelState.Remove("modelo.Senha");
            ModelState.Remove("modelo.NovaSenha");
            ModelState.Remove("modelo.ConfirmaNovaSenha");

            //Validando o registro
            if (modelo == null)
            {
                ModelState.AddModelError("", "Error");
                return BadRequest(ModelState);
            }

            //Verifica o tipo de registro do Usuário (não está sendo mais usado)
            if (!modelo.TipoUsuario)
            {
                ModelState.Remove("modelo.usuarioRG");
                ModelState.Remove("modelo.usuarioCPF");
                ModelState.Remove("modelo.usuarioEmail");
            }

            //Verificando se o modelo é válido
            if (!ModelState.IsValid)
                return BadRequest("Preencha os campos obrigatorios marcados com  *  ");

            //Verifica Login
            int qtd = _dbContext.Usuario.Count(x => x.LOGIN == modelo.usuarioLogin);
            if (qtd > 0 && modelo.usuarioID == 0)
                return BadRequest("Login já cadastrado");

            //Verifica CPF
            int cpf = _dbContext.Usuario.Count(x => x.CPF == modelo.usuarioCPF);
            if (cpf > 0 && modelo.usuarioID == 0)
                return BadRequest("CPF já cadastrado");

            // Recupera o ID do usuário que está cadastrando ou alterando um usuário
            int idAdmin = Convert.ToInt32(_authenticationManager.User.Identity.Name);
            Usuario admin = _dbContext.Usuario.FirstOrDefault(x => x.ID_USUARIO == idAdmin && x.FLAG_ATIVO);

            //Novo usuário
            if (modelo.usuarioID == 0)
            {
                //Criando o SALT
                RandomNumberGenerator rng = RandomNumberGenerator.Create();
                byte[] number = new byte[32];
                rng.GetBytes(number);
                string pass_salt = Convert.ToBase64String(number);

                //Criando o hash da primeira senha (CPF) do novo usuário
                crypt = SHA256Managed.Create();

                string senha = "depa123";
                if (!String.IsNullOrWhiteSpace(modelo.usuarioCPF))
                    senha = modelo.usuarioCPF;

                string pass = senha;
                byte[] passbytes = Encoding.UTF8.GetBytes(pass_salt + pass);
                byte[] hash = crypt.ComputeHash(passbytes);

                Usuario novo = new Usuario()
                {
                    ID_USUARIO = modelo.usuarioID,
                    LOGIN = modelo.usuarioLogin,
                    NOME_COMPLETO = modelo.usuarioNome,
                    EMAIL = modelo.usuarioEmail,
                    CPF = modelo.usuarioCPF,
                    RG = modelo.usuarioRG,
                    RG_UF = modelo.usuarioRG_UF,
                    ID_GRUPO = modelo.GrupoId,
                    DATA_ALTERADO = DateTime.Now,
                    DATA_CADASTRO = DateTime.Now,
                    FLAG_ATIVO = true,
                    PRIMEIRO_ACESSO = true,
                    SALT = pass_salt,
                    SENHA = Convert.ToBase64String(hash),
                };
                _dbContext.Usuario.Add(novo);

                UsuarioLog ulog = new UsuarioLog();
                ulog.ID_USUARIO = idAdmin;
                ulog.DATA_LOG = DateTime.Now;
                ulog.DESCRICAO_LOG = string.Format("O administrador '{0}' realizou o cadastro de um novo usuário.", admin.NOME_COMPLETO);
                _dbContext.UsuarioLog.Add(ulog);

                _dbContext.SaveChanges();

                // Verifica se o retorno do usuário salvo está OK
                if (novo.ID_USUARIO > 0)
                {
                    // Verifica se existem unidades selecionadas para o usuário
                    if (modelo.Orgaos != null && modelo.Orgaos.Count > 0)
                    {
                        UsuarioOrgaos orgao;

                        // Grava as informações na tabela de relacionamento
                        foreach (int id in modelo.Orgaos)
                        {
                            orgao = new UsuarioOrgaos();
                            orgao.ID_USUARIO = novo.ID_USUARIO;
                            orgao.ID_ORGAO = id;
                            orgao.FLAG_ATIVO = true;

                            _dbContext.UsuarioOrgaos.Add(orgao);
                        }

                        //Salva as informações
                        _dbContext.SaveChanges();
                    }
                }

                return Ok("Usuário cadastrado com sucesso!");
            }

            //Editando usuário
            if (modelo.usuarioID > 0)
            {
                //Recuperando o usuário
                Usuario user = _dbContext.Usuario.FirstOrDefault(c => c.ID_USUARIO == modelo.usuarioID);
                user.NOME_COMPLETO = modelo.usuarioNome;
                user.EMAIL = modelo.usuarioEmail;
                user.CPF = modelo.usuarioCPF;
                user.RG = modelo.usuarioRG;
                user.RG_UF = modelo.usuarioRG_UF;
                user.ID_GRUPO = modelo.GrupoId;
                user.DATA_ALTERADO = DateTime.Now;

                //Verificando a lista de orgãos para alteração
                List<UsuarioOrgaos> orgaos = _dbContext.UsuarioOrgaos.Where(o => o.ID_USUARIO == modelo.usuarioID).ToList();

                //Desativando os orgãos cadastrados no banco que não foram selecionados e estão ativos
                foreach (UsuarioOrgaos uo in orgaos.Where(x => !modelo.Orgaos.Any(o => o == x.ID_ORGAO) && x.FLAG_ATIVO))
                    uo.FLAG_ATIVO = false;

                //Ativando os orgãos que existem no banco e estão inativos
                foreach (UsuarioOrgaos uo in orgaos.Where(x => modelo.Orgaos.Any(o => o == x.ID_ORGAO) && !x.FLAG_ATIVO))
                    uo.FLAG_ATIVO = true;

                //Registra os novos orgãos marcados que não estão no banco
                UsuarioOrgaos novo;
                foreach (int id in modelo.Orgaos.Where(x => !orgaos.Any(o => o.ID_ORGAO == x)).ToList())
                {
                    novo = new UsuarioOrgaos();
                    novo.ID_USUARIO = modelo.usuarioID;
                    novo.ID_ORGAO = id;
                    novo.FLAG_ATIVO = true;
                    _dbContext.UsuarioOrgaos.Add(novo);
                }

                //Gerando registro de log
                UsuarioLog ulog = new UsuarioLog();
                ulog.ID_USUARIO = idAdmin;
                ulog.ID_USUARIO_ALTERADO = modelo.usuarioID;
                ulog.DATA_LOG = DateTime.Now;
                ulog.DESCRICAO_LOG = string.Format("O administrador '{0}' alterou o cadastro do usuário '{1}'.", admin.NOME_COMPLETO, user.NOME_COMPLETO);
                _dbContext.UsuarioLog.Add(ulog);

                //Salvando as informações em banco
                _dbContext.SaveChanges();

                return Ok("Usuário alterado com sucesso!");
            }

            return BadRequest(ModelState);
        }

        [HttpGet]
        public IHttpActionResult CarregarOrgaosUsuario(int id)
        {
            try
            {
                List<int> lista = _dbContext.UsuarioOrgaos.AsNoTracking().Where(x => x.ID_USUARIO == id && x.FLAG_ATIVO).Select(o => o.ID_ORGAO).ToList();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        public IHttpActionResult verificaUsuarioCPF(string usuarioCPF)
        {
            try
            {
                int usuarios = _dbContext.Usuario.Count(x => x.CPF == usuarioCPF);

                if (usuarios > 0)
                    return Ok(false);
                else
                    return Ok(true);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return BadRequest(ModelState);
            }
        }

    }
}
