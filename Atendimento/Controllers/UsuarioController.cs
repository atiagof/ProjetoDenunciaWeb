using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebDenunciaSSP.Atendimento.Models;
using WebDenunciaSSP.Entidades;
using WebDenunciaSSP.Entidades.Context;

namespace WebDenunciaSSP.Atendimento.Controllers
{


    public class UsuarioController : Controller
    {

        private readonly WDContext _dbContext;
        private SHA256 crypt;

        public UsuarioController(WDContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Usuario
        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar")]
        public ActionResult ListaUsuario()
        {
            return View();
        }

        // GET: Usuario
        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar,Atendimento Civil,Atendimento Militar,Policia Civil,Policia Militar")]
        public ActionResult ResetarSenha()
        {
            return View();
        }

        // GET: Usuario
        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar")]
        public ActionResult RegistroOLD(int? id)
        {
            RegistroUsuarioViewModel model = new RegistroUsuarioViewModel();
                      
            if(id.HasValue)
            {
                Usuario usuario = _dbContext.Usuario.FirstOrDefault(x => x.ID_USUARIO == id);

                model.usuarioID = usuario.ID_USUARIO;
                model.usuarioNome = usuario.NOME_COMPLETO;
                model.usuarioLogin = usuario.LOGIN;
                model.usuarioCPF = usuario.CPF;
                model.usuarioEmail = usuario.EMAIL;
                model.usuarioRG = usuario.RG;
                model.usuarioRG_UF = usuario.RG_UF;

                model.GrupoId = usuario.ID_GRUPO;
                model.OrgaoId = usuario.ID_ORGAO;

                if (model.OrgaoId > 0)
                {
                    int orgaoid = Convert.ToInt16(model.OrgaoId);

                    Orgao orgao = _dbContext.Orgao.FirstOrDefault(c => c.ID_ORGAO == orgaoid);

                    SubtipoOrgao subtipoOrgao = _dbContext.SubtipoOrgao.FirstOrDefault(c => c.ID_SUBTIPO_ORGAO == orgao.ID_SUBTIPO_ORGAO);

                    model.OrgaoIdSubTipo = subtipoOrgao.ID_SUBTIPO_ORGAO;

                    TipoOrgao tipoOrgao = _dbContext.TipoOrgao.FirstOrDefault(c => c.ID_TIPO_ORGAO == subtipoOrgao.ID_TIPO_ORGAO);

                    model.OrgaoIdTipo = tipoOrgao.ID_TIPO_ORGAO;
                }
                
                if (usuario.CPF == "")
                    model.TipoUsuario = true;
            }

            return View(model);
        }

        // GET: Usuario
        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar")]
        public ActionResult Registro(int? id)
        {
            RegistroUsuarioViewModel model = new RegistroUsuarioViewModel();

            if (id.HasValue)
            {
                Usuario usuario = _dbContext.Usuario.FirstOrDefault(x => x.ID_USUARIO == id);

                model.usuarioID = usuario.ID_USUARIO;
                model.usuarioNome = usuario.NOME_COMPLETO;
                model.usuarioLogin = usuario.LOGIN;
                model.usuarioCPF = usuario.CPF;
                model.usuarioEmail = usuario.EMAIL;
                model.usuarioRG = usuario.RG;
                model.usuarioRG_UF = usuario.RG_UF;
                model.GrupoId = usuario.ID_GRUPO;

                if (usuario.CPF == "")
                    model.TipoUsuario = true;
            }

            return View(model);
        }

    }
}