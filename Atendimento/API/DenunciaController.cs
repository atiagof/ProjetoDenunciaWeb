using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.ServiceModel.Channels;
using WebDenunciaSSP.Atendimento.Models;
using WebDenunciaSSP.CommonLibs.Models;
using WebDenunciaSSP.Entidades;
using WebDenunciaSSP.Entidades.Context;
using System.Text;

namespace WebDenunciaSSP.Atendimento.API
{
    [Authorize]
    public class DenunciaController : ApiController
    {
        private readonly WDContext _dbContext;

        public DenunciaController(WDContext context)
        {
            _dbContext = context;
        }

        #region GET

        [HttpGet]
        // Abre as informações por completo da denúncia
        public IHttpActionResult abrirDenunciaCompleto(int ano, int numero)
        {
            try
            {
                DenunciaModel denObj = new DenunciaModel();

                Denuncia den = _dbContext.Denuncia
                    .Include("DenunciaAnexo") // Lista de anexos
                    .Include("DenunciaAnimais") // Lista de animais
                    .Include("DenunciaLink") // Lista de endereços eletrônicos
                    .Include("DenunciaPessoa") // Lista de pessoas
                    .Include("DenunciaVeiculo") // Lista de veículos
                    .Include("Periodo") // Período
                    .Include("TipoCrime") // Tipo de crime
                    .Include("Situacao") // Situação PC
                    .Include("Situacao1") // Situação PM
                    .Include("TipoEndereco") // Tipo de endereço da ocorrência
                    .Include("LocalFuga") // Possível local de fuga marcado na denúncia
                    .Include("Orgao") // Orgão de encaminhamento PC
                    .Include("Orgao1") // Orgão de encamminhamento PM
                    .Include("OrgaoElaboracaoPC") // Delegacia que elaborou o BO (lado PC)
                    .Include("OrgaoElaboracaoPM") // Delegacia que elaborou o BO (lado PM)
                    .Include("TipoBoletimPC") // Tipo de Boletim PC
                    .Include("TipoBoletimPM") // Tipo de Boletim PM
                    .Include("DenunciaRespostas") // Retorna o histórico de respostas da Denúncia
                    .Include("Usuario6") // Usuário Atendimento PC
                    .Include("Usuario7") // Usuário Atendimento PM
                    .Include("Usuario8") // Usuário Descarte PC
                    .Include("Usuario9") // Usuário Descarte PM
                    .Include("Usuario10") // Usuário Reativação PC
                    .Include("Usuario11") // Usuário Reativação PM
                    .Include("Usuario12") // Usuário Resposta PC
                    .Include("Usuario13") // Usuário Resposta PM
                    .Include("Usuario14") // Usuário Validação PC
                    .Include("Usuario15") // Usuário Validação PM
                    .Include("Usuario16") // Usuário Retorno PC
                    .Include("Usuario17") // Usuário Retorno PM
                    .FirstOrDefault(d => d.ANO_DENUNCIA == ano && d.NUM_DENUNCIA == numero); // Recupera a denúncia pela chave principal

                if (den != null)
                {
                    // Dados da denúncia
                    denObj.ano = den.ANO_DENUNCIA;
                    denObj.numero = den.NUM_DENUNCIA;
                    denObj.periodo = den.Periodo.DESCRICAO_PERIODO;
                    denObj.data_cadastro = den.DATA_CADASTRO.ToString("dd/MM/yyyy HH:mm:ss");
                    denObj.data_ocorrencia = den.DATA_OCORRENCIA.ToString("dd/MM/yyyy");
                    denObj.data_atendimento_pc = den.DATA_ATENDIMENTO_PC.HasValue ? den.DATA_ATENDIMENTO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_encaminhamento_pc = den.DATA_ENCAMINHAMENTO_PC.HasValue ? den.DATA_ENCAMINHAMENTO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_resposta_pc = den.DATA_RESPOSTA_PC.HasValue ? den.DATA_RESPOSTA_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_validacao_pc = den.DATA_VALIDACAO_PC.HasValue ? den.DATA_VALIDACAO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_descarte_pc = den.DATA_DESCARTE_PC.HasValue ? den.DATA_DESCARTE_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_reativo_pc = den.DATA_REATIVO_PC.HasValue ? den.DATA_REATIVO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_retorno_pc = den.DATA_RETORNO_PC.HasValue ? den.DATA_RETORNO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_providencia_pc = den.DATA_PROVIDENCIA_PC.HasValue ? den.DATA_PROVIDENCIA_PC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    denObj.data_atendimento_pm = den.DATA_ATENDIMENTO_PM.HasValue ? den.DATA_ATENDIMENTO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_encaminhamento_pm = den.DATA_ENCAMINHAMENTO_PM.HasValue ? den.DATA_ENCAMINHAMENTO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_resposta_pm = den.DATA_RESPOSTA_PM.HasValue ? den.DATA_RESPOSTA_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_validacao_pm = den.DATA_VALIDACAO_PM.HasValue ? den.DATA_VALIDACAO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_descarte_pm = den.DATA_DESCARTE_PM.HasValue ? den.DATA_DESCARTE_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_reativo_pm = den.DATA_REATIVO_PM.HasValue ? den.DATA_REATIVO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_retorno_pm = den.DATA_RETORNO_PM.HasValue ? den.DATA_RETORNO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_providencia_pm = den.DATA_PROVIDENCIA_PM.HasValue ? den.DATA_PROVIDENCIA_PM.Value.ToString("dd/MM/yyyy") : string.Empty;
                    denObj.flag_tipo_crime = den.TipoCrime.ID_TIPO_CRIME == 6;
                    denObj.tipo_crime = den.TipoCrime.DESCRICAO_TIPO_CRIME;
                    denObj.outro_tipo_crime = den.OUTRO_TIPO_CRIME;
                    denObj.relato = den.RELATO;
                    denObj.id_status_pc = den.Situacao.ID_SITUACAO;
                    denObj.status_pc = den.Situacao.DESCRICAO_SITUACAO;
                    denObj.id_status_pm = den.Situacao1.ID_SITUACAO;
                    denObj.status_pm = den.Situacao1.DESCRICAO_SITUACAO;
                    denObj.just_descarte_pc = den.JUSTIFICATIVA_DESCARTE_PC;
                    denObj.just_reativo_pc = den.JUSTIFICATIVA_REATIVO_PC;
                    denObj.just_retorno_pc = den.JUSTIFICATIVA_RETORNO_PC;
                    denObj.just_descarte_pm = den.JUSTIFICATIVA_DESCARTE_PM;
                    denObj.just_reativo_pm = den.JUSTIFICATIVA_REATIVO_PM;
                    denObj.just_retorno_pm = den.JUSTIFICATIVA_RETORNO_PM;
                    denObj.resposta_pc = den.RESPOSTA_PC;
                    denObj.resposta_final_pc = den.RESPOSTA_FINAL_PC;
                    denObj.resposta_pm = den.RESPOSTA_PM;
                    denObj.resposta_final_pm = den.RESPOSTA_FINAL_PM;
                    denObj.tipo_endereco = den.TipoEndereco.DESCRICAO_TIPO_ENDERECO;
                    denObj.cep = den.LOGRADOURO_CEP;
                    denObj.endereco = den.LOGRADOURO;
                    denObj.endereco_numero = den.LOGRADOURO_NUMERO;
                    denObj.complemento = den.LOGRADOURO_COMPLEMENTO;
                    denObj.bairro = den.LOGRADOURO_BAIRRO;
                    denObj.cidade = den.CIDADE;
                    denObj.estado = den.ESTADO;
                    denObj.endereco_referencia = den.PONTO_REFERENCIA;
                    denObj.id_orgao_pc = den.Orgao != null ? den.Orgao.ID_ORGAO : 0;
                    denObj.id_orgao_pm = den.Orgao1 != null ? den.Orgao1.ID_ORGAO : 0;
                    denObj.orgao_pc = den.Orgao != null ? den.Orgao.NOME_ORGAO : string.Empty;
                    denObj.orgao_pm = den.Orgao1 != null ? den.Orgao1.NOME_ORGAO : string.Empty;
                    denObj.usuario_atendimento_pc = den.Usuario6 != null ? den.Usuario6.NOME_COMPLETO : string.Empty;
                    denObj.usuario_atendimento_pm = den.Usuario7 != null ? den.Usuario7.NOME_COMPLETO : string.Empty;
                    denObj.usuario_descarte_pc = den.Usuario8 != null ? den.Usuario8.NOME_COMPLETO : string.Empty;
                    denObj.usuario_descarte_pm = den.Usuario9 != null ? den.Usuario9.NOME_COMPLETO : string.Empty;
                    denObj.usuario_reativo_pc = den.Usuario10 != null ? den.Usuario10.NOME_COMPLETO : string.Empty;
                    denObj.usuario_reativo_pm = den.Usuario11 != null ? den.Usuario11.NOME_COMPLETO : string.Empty;
                    denObj.usuario_resposta_pc = den.Usuario12 != null ? den.Usuario12.NOME_COMPLETO : string.Empty;
                    denObj.usuario_resposta_pm = den.Usuario13 != null ? den.Usuario13.NOME_COMPLETO : string.Empty;
                    denObj.usuario_validacao_pc = den.Usuario14 != null ? den.Usuario14.NOME_COMPLETO : string.Empty;
                    denObj.usuario_validacao_pm = den.Usuario15 != null ? den.Usuario15.NOME_COMPLETO : string.Empty;
                    denObj.usuario_validacao_pc = den.Usuario16 != null ? den.Usuario16.NOME_COMPLETO : string.Empty;
                    denObj.usuario_validacao_pm = den.Usuario17 != null ? den.Usuario17.NOME_COMPLETO : string.Empty;
                    denObj.delegacia_elaboracao_pc = den.OrgaoElaboracaoPC != null ? den.OrgaoElaboracaoPC.NOME_ORGAO : string.Empty;
                    denObj.tipo_boletim_pc = den.TipoBoletimPC != null ? den.TipoBoletimPC.DESCRICAO_TIPO_BOLETIM : string.Empty;
                    denObj.numero_boletim_pc = den.NUMERO_BOLETIM_PC;
                    denObj.flag_resposta_pc = den.FLAG_RESPOSTA_POSITIVA_PC.HasValue ? (den.FLAG_RESPOSTA_POSITIVA_PC.Value ? "Sim" : "Não") : string.Empty;
                    denObj.flag_registroBO_pc = den.ID_DELEGACIA_ELABORACAO_PC.HasValue ? "Sim" : "Não";
                    denObj.delegacia_elaboracao_pm = den.OrgaoElaboracaoPM != null ? den.OrgaoElaboracaoPM.NOME_ORGAO : string.Empty;
                    denObj.tipo_boletim_pm = den.TipoBoletimPM != null ? den.TipoBoletimPM.DESCRICAO_TIPO_BOLETIM : string.Empty;
                    denObj.numero_boletim_pm = den.NUMERO_BOLETIM_PM;
                    denObj.flag_resposta_pm = den.FLAG_RESPOSTA_POSITIVA_PM.HasValue ? (den.FLAG_RESPOSTA_POSITIVA_PM.Value ? "Sim" : "Não") : string.Empty;
                    denObj.flag_registroBO_pm = den.ID_DELEGACIA_ELABORACAO_PM.HasValue ? "Sim" : "Não";
                    denObj.flag_boletim = den.FLAG_BOLETIM.HasValue ? (den.FLAG_BOLETIM.Value ? "Sim" : "Não") : "Não sei";
                    denObj.flag_porte_arma = den.FLAG_PORTE_ARMA_FOGO.HasValue ? (den.FLAG_PORTE_ARMA_FOGO.Value ? "Sim" : "Não") : "Não sei";
                    denObj.flag_local_fuga = den.LocalFuga != null ? den.LocalFuga.ID_LOCAL_FUGA == 5 : false;
                    denObj.local_fuga = den.LocalFuga != null ? den.LocalFuga.DESCRICAO_LOCAL_FUGA : string.Empty;
                    denObj.descricao_local_fuga = den.DESCRICAO_LOCAL_FUGA;

                    // Dados do denunciante e denunciados (Informações, Endereço, Telefone e Características)
                    if (den.DenunciaPessoa.Count > 0)
                    {
                        DenunciaPessoa denunciante = den.DenunciaPessoa.FirstOrDefault(dp => dp.ID_TIPO_PESSOA == 1);

                        PessoaTelefone resDenunciante = null;
                        PessoaTelefone celDenunciante = null;
                        PessoaEndereco endDenunciante = null;

                        if (denunciante.PessoaTelefone.Count > 0)
                        {
                            resDenunciante = denunciante.PessoaTelefone.FirstOrDefault(t => t.ID_TIPO_TELEFONE == 1 && t.ID_PESSOA == denunciante.ID_PESSOA);
                            celDenunciante = denunciante.PessoaTelefone.FirstOrDefault(t => t.ID_TIPO_TELEFONE == 3 && t.ID_PESSOA == denunciante.ID_PESSOA);
                        }

                        if (denunciante.PessoaEndereco.Count > 0)
                            endDenunciante = denunciante.PessoaEndereco.FirstOrDefault(e => e.ID_PESSOA == denunciante.ID_PESSOA);

                        denObj.denunciante = new DenuncianteModel()
                        {
                            nome = denunciante.NOME_PESSOA,
                            sexo = denunciante.Sexo != null ? denunciante.Sexo.DESCRICAO_SEXO : string.Empty,
                            tel_residencial = resDenunciante != null ? resDenunciante.PESSOA_TELEFONE : string.Empty,
                            tel_celular = celDenunciante != null ? celDenunciante.PESSOA_TELEFONE : string.Empty,
                            email = denunciante.EMAIL,
                            CPF = denunciante.CPF,
                            RG = denunciante.RG,
                            RG_UF = denunciante.RG_UF,
                            naturalidade = denunciante.NATURALIDADE,
                            data_nascimento = denunciante.DATA_NASCIMENTO.HasValue ? denunciante.DATA_NASCIMENTO.Value.ToString("dd/MM/yyyy") : string.Empty,
                            estado_civil = denunciante.EstadoCivil != null ? denunciante.EstadoCivil.DESCRICAO_ESTADO_CIVIL : string.Empty,
                            profissao = denunciante.PROFISSAO,
                            sigilo = denunciante.FLAG_SIGILO.HasValue ? (denunciante.FLAG_SIGILO.Value ? "Sim" : "Não") : string.Empty,
                            CEP = endDenunciante.LOGRADOURO_CEP,
                            endereco = endDenunciante.LOGRADOURO,
                            numero = endDenunciante.LOGRADOURO_NUMERO,
                            complemento = endDenunciante.LOGRADOURO_COMPLEMENTO,
                            bairro = endDenunciante.LOGRADOURO_BAIRRO,
                            cidade = endDenunciante.CIDADE,
                            estado = endDenunciante.ESTADO,
                            referencia = endDenunciante.PONTO_REFERENCIA,
                        };

                        denObj.denunciados = new List<DenunciadoModel>();
                        List<DenunciaPessoa> denunciados = den.DenunciaPessoa.Where(dp => dp.ID_TIPO_PESSOA == 2).ToList();

                        foreach (DenunciaPessoa dp in denunciados)
                        {
                            PessoaCaracteristicas carac = null;
                            PessoaEndereco endDenunciado = null;

                            if (dp.PessoaCaracteristicas.Count > 0)
                                carac = dp.PessoaCaracteristicas.FirstOrDefault(pc => pc.ID_PESSOA == dp.ID_PESSOA);

                            if (dp.PessoaEndereco.Count > 0)
                                endDenunciado = dp.PessoaEndereco.FirstOrDefault(ed => ed.ID_PESSOA == dp.ID_PESSOA);

                            denObj.denunciados.Add(
                                new DenunciadoModel()
                                {
                                    nome = dp.NOME_PESSOA,
                                    apelido = dp.APELIDO_PESSOA,
                                    sexo = carac.Sexo != null ? carac.Sexo.DESCRICAO_SEXO : string.Empty,
                                    idade = carac.FaixaEtaria != null ? carac.FaixaEtaria.DESCRICAO_FAIXA_ETARIA : string.Empty,
                                    estatura = carac.Estatura != null ? carac.Estatura.DESCRICAO_ESTATURA : string.Empty,
                                    tipo_fisico = carac.TipoFisico != null ? carac.TipoFisico.DESCRICAO_TIPO_FISICO : string.Empty,
                                    cor_pele = carac.CorPele != null ? carac.CorPele.DESCRICAO_COR : string.Empty,
                                    cor_olhos = carac.CorOlhos != null ? carac.CorOlhos.DESCRICAO_COR : string.Empty,
                                    flag_tipo_cabelo = carac.TipoCabelo != null ? carac.TipoCabelo.ID_TIPO_CABELO == 9 : false,
                                    tipo_cabelo = carac.TipoCabelo != null ? carac.TipoCabelo.DESCRICAO_TIPO_CABELO : string.Empty,
                                    desc_tipo_cabelo = carac.TipoCabelo != null ? (carac.TipoCabelo.ID_TIPO_CABELO == 9 ? carac.DESCRICAO_TIPO_CABELO : carac.TipoCabelo.DESCRICAO_TIPO_CABELO) : string.Empty,
                                    flag_cor_cabelo = carac.CorCabelo != null ? carac.CorCabelo.ID_COR_CABELO == 7 : false,
                                    cor_cabelo = carac.CorCabelo != null ? carac.CorCabelo.DESCRICAO_COR : string.Empty,
                                    desc_cor_cabelo = carac.CorCabelo != null ? (carac.CorCabelo.ID_COR_CABELO == 7 ? carac.DESCRICAO_COR_CABELO : carac.CorCabelo.DESCRICAO_COR) : string.Empty,
                                    flag_cicatriz = carac.Cicatriz != null ? (carac.Cicatriz.ID_CICATRIZ == 1) : false,
                                    cicatriz = carac.Cicatriz != null ? carac.Cicatriz.DESCRICAO_CICATRIZ : string.Empty,
                                    desc_cicatriz = carac.Cicatriz != null ? (carac.Cicatriz.ID_CICATRIZ == 1 ? carac.DESCRICAO_CICATRIZ : string.Empty) : string.Empty,
                                    flag_tatuagem = carac.Tatuagem != null ? (carac.Tatuagem.ID_TATUAGEM == 1) : false,
                                    tatuagem = carac.Tatuagem != null ? carac.Tatuagem.DESCRICAO_TATUAGEM : string.Empty,
                                    desc_tatuagem = carac.Tatuagem != null ? (carac.Tatuagem.ID_TATUAGEM == 1 ? carac.DESCRICAO_TATUAGEM : string.Empty) : string.Empty,
                                    flag_endereco = endDenunciado != null,
                                    tipo_endereco = endDenunciado == null ? string.Empty : endDenunciado.TipoEndereco.DESCRICAO_TIPO_ENDERECO,
                                    CEP = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO_CEP,
                                    endereco = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO,
                                    numero = endDenunciado == null ? 0 : endDenunciado.LOGRADOURO_NUMERO,
                                    complemento = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO_COMPLEMENTO,
                                    bairro = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO_BAIRRO,
                                    cidade = endDenunciado == null ? string.Empty : endDenunciado.CIDADE,
                                    estado = endDenunciado == null ? string.Empty : endDenunciado.ESTADO,
                                    referencia = endDenunciado == null ? string.Empty : endDenunciado.PONTO_REFERENCIA,
                                }
                            );
                        }

                    }

                    // Dados dos animais
                    if (den.DenunciaAnimais.Count > 0)
                    {
                        denObj.animais = new List<AnimalModel>();
                        den.DenunciaAnimais.ToList().ForEach(da =>
                            denObj.animais.Add(new AnimalModel()
                            {
                                tipo_animal = da.TipoAnimal.ID_TIPO_ANIMAL == 7 ? string.Format("{0} - {1}", da.TipoAnimal.DESCRICAO_TIPO_ANIMAL, da.OUTRO_TIPO_ANIMAL) : da.TipoAnimal.DESCRICAO_TIPO_ANIMAL,
                                porte_animal = da.PorteAnimal.DESCRICAO_PORTE_ANIMAL,
                                quantidade = da.QUANTIDADE,
                            })
                        );
                    }

                    // Dados dos veículos
                    if (den.DenunciaVeiculo.Count > 0)
                    {
                        denObj.veiculos = new List<VeiculoModel>();
                        den.DenunciaVeiculo.ToList().ForEach(dv =>
                            denObj.veiculos.Add(new VeiculoModel()
                            {
                                tipo_veiculo = dv.TipoVeiculo.DESCRICAO_TIPO_VEICULO,
                                placa = dv.NUMERO_PLACA,
                                marca = dv.MARCA_VEICULO,
                                cor_veiculo = dv.CorVeiculo.DESCRICAO_COR,
                                modelo = dv.MODELO_VEICULO,
                                observacoes = dv.OBSERVACOES,
                            })
                        );
                    }

                    // Dados dos anexos
                    if (den.DenunciaAnexo.Count > 0)
                    {
                        denObj.anexos = new List<AnexoModel>();
                        den.DenunciaAnexo.ToList().ForEach(da =>
                            denObj.anexos.Add(new AnexoModel()
                            {
                                endereco = da.ENDERECO_ANEXO,
                                nome = da.ID_ANEXO.ToString(),
                                original = da.NOME_ORIGINAL,
                                extensao = da.MIME_TYPE,
                                observacoes = da.OBSERVACOES,
                                preview = (new List<string>() { ".bmp", ".gif", ".jpe", ".jpeg", ".jpg", ".png", ".tif" }.Any(t => t == da.MIME_TYPE)),
                            })
                        );
                    }

                    // Dados das páginas de Internet
                    if (den.DenunciaLink.Count > 0)
                    {
                        denObj.links = new List<LinkModel>();
                        den.DenunciaLink.ToList().ForEach(dl =>
                            denObj.links.Add(new LinkModel()
                            {
                                endereco = dl.ENDERECO_LINK,
                                observacoes = dl.OBSERVACOES,
                            })
                        );
                    }

                    // Histórico de respostas
                    if (den.DenunciaRespostas.Count > 0)
                    {
                        List<DenunciaRespostas> civil = den.DenunciaRespostas.Where(r => r.FLAG_CIVIL).ToList();
                        List<DenunciaRespostas> militar = den.DenunciaRespostas.Where(r => r.FLAG_MILITAR).ToList();

                        denObj.historico_pc = new List<RespostaModel>();
                        civil.ForEach(rc => denObj.historico_pc.Add(new RespostaModel()
                        {
                            resposta = rc.RESPOSTA,
                            usuario_resposta = rc.UsuarioResposta.NOME_COMPLETO,
                            just_retorno = rc.JUSTIFICATIVA_RETORNO,
                            usuario_retorno = rc.UsuarioRetorno != null ? rc.UsuarioRetorno.NOME_COMPLETO : string.Empty,
                            data_resposta = rc.DATA_RESPOSTA.ToString("dd/MM/yyyy HH:mm:ss"),
                            data_retorno = rc.DATA_RETORNO.HasValue ? rc.DATA_RETORNO.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        })
                        );

                        denObj.historico_pm = new List<RespostaModel>();
                        militar.ForEach(rm => denObj.historico_pm.Add(new RespostaModel()
                        {
                            resposta = rm.RESPOSTA,
                            usuario_resposta = rm.UsuarioResposta.NOME_COMPLETO,
                            just_retorno = rm.JUSTIFICATIVA_RETORNO,
                            usuario_retorno = rm.UsuarioRetorno != null ? rm.UsuarioRetorno.NOME_COMPLETO : string.Empty,
                            data_resposta = rm.DATA_RESPOSTA.ToString("dd/MM/yyyy HH:mm:ss"),
                            data_retorno = rm.DATA_RETORNO.HasValue ? rm.DATA_RETORNO.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        })
                        );
                    }
                }

                return Ok(denObj);
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("Um erro ocorreu ao carregar os dados completos da denúncia:")
                .AppendLine(ex.ToString());
                LogErrorModel log = new Models.LogErrorModel(Request, error.ToString());

                PreenchimentoLog exc = new PreenchimentoLog()
                {
                    DATA_LOG = log.data,
                    NAVEGADOR = log.browser,
                    IP_CADASTRO = log.ip,
                    DESCRICAO_LOG = log.log,
                };
                _dbContext.PreenchimentoLog.Add(exc);
                _dbContext.SaveChanges();

                ModelState.AddModelError("", ex);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        // Abre as informações da denúncia
        public IHttpActionResult abrirDenuncia(int ano, int numero, bool atendimento)
        {
            try
            {
                DenunciaModel denObj = new DenunciaModel();

                Denuncia den = _dbContext.Denuncia
                    .Include("DenunciaAnexo") // Lista de anexos
                    .Include("DenunciaAnimais") // Lista de animais
                    .Include("DenunciaLink") // Lista de endereços eletrônicos
                    .Include("DenunciaPessoa") // Lista de pessoas
                    .Include("DenunciaVeiculo") // Lista de veículos
                    .Include("Periodo") // Período
                    .Include("TipoCrime") // Tipo de crime
                    .Include("Situacao") // Situação PC
                    .Include("Situacao1") // Situação PM
                    .Include("TipoEndereco") // Tipo de endereço da ocorrência
                    .Include("LocalFuga") // Possível local de fuga marcado na denúncia
                    .Include("Orgao") // Orgão de encaminhamento PC
                    .Include("Orgao1") // Orgão de encamminhamento PM
                    .Include("OrgaoElaboracaoPC") // Delegacia que elaborou o BO (lado PC)
                    .Include("OrgaoElaboracaoPM") // Delegacia que elaborou o BO (lado PM)
                    .Include("TipoBoletimPC") // Tipo de Boletim PC
                    .Include("TipoBoletimPM") // Tipo de Boletim PM
                    .Include("DenunciaRespostas") // Retorna o histórico de respostas da Denúncia
                    .Include("Usuario6") // Usuário Atendimento PC
                    .Include("Usuario7") // Usuário Atendimento PM
                    .Include("Usuario8") // Usuário Descarte PC
                    .Include("Usuario9") // Usuário Descarte PM
                    .Include("Usuario10") // Usuário Reativação PC
                    .Include("Usuario11") // Usuário Reativação PM
                    .Include("Usuario12") // Usuário Resposta PC
                    .Include("Usuario13") // Usuário Resposta PM
                    .Include("Usuario14") // Usuário Validação PC
                    .Include("Usuario15") // Usuário Validação PM
                    .Include("Usuario16") // Usuário Retorno PC
                    .Include("Usuario17") // Usuário Retorno PM
                    .FirstOrDefault(d => d.ANO_DENUNCIA == ano && d.NUM_DENUNCIA == numero); // Recupera a denúncia pela chave principal

                if (den != null)
                {
                    denObj.ano = den.ANO_DENUNCIA;
                    denObj.numero = den.NUM_DENUNCIA;
                    denObj.periodo = den.Periodo.DESCRICAO_PERIODO;
                    denObj.data_cadastro = den.DATA_CADASTRO.ToString("dd/MM/yyyy HH:mm:ss");
                    denObj.data_ocorrencia = den.DATA_OCORRENCIA.ToString("dd/MM/yyyy");
                    denObj.data_atendimento_pc = den.DATA_ATENDIMENTO_PC.HasValue ? den.DATA_ATENDIMENTO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_encaminhamento_pc = den.DATA_ENCAMINHAMENTO_PC.HasValue ? den.DATA_ENCAMINHAMENTO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_resposta_pc = den.DATA_RESPOSTA_PC.HasValue ? den.DATA_RESPOSTA_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_validacao_pc = den.DATA_VALIDACAO_PC.HasValue ? den.DATA_VALIDACAO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_descarte_pc = den.DATA_DESCARTE_PC.HasValue ? den.DATA_DESCARTE_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_reativo_pc = den.DATA_REATIVO_PC.HasValue ? den.DATA_REATIVO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_retorno_pc = den.DATA_RETORNO_PC.HasValue ? den.DATA_RETORNO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_providencia_pc = den.DATA_PROVIDENCIA_PC.HasValue ? den.DATA_PROVIDENCIA_PC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    denObj.data_atendimento_pm = den.DATA_ATENDIMENTO_PM.HasValue ? den.DATA_ATENDIMENTO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_encaminhamento_pm = den.DATA_ENCAMINHAMENTO_PM.HasValue ? den.DATA_ENCAMINHAMENTO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_resposta_pm = den.DATA_RESPOSTA_PM.HasValue ? den.DATA_RESPOSTA_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_validacao_pm = den.DATA_VALIDACAO_PM.HasValue ? den.DATA_VALIDACAO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_descarte_pm = den.DATA_DESCARTE_PM.HasValue ? den.DATA_DESCARTE_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_reativo_pm = den.DATA_REATIVO_PM.HasValue ? den.DATA_REATIVO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_retorno_pm = den.DATA_RETORNO_PM.HasValue ? den.DATA_RETORNO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    denObj.data_providencia_pm = den.DATA_PROVIDENCIA_PM.HasValue ? den.DATA_PROVIDENCIA_PM.Value.ToString("dd/MM/yyyy") : string.Empty;
                    denObj.flag_tipo_crime = den.TipoCrime.ID_TIPO_CRIME == 6;
                    denObj.tipo_crime = den.TipoCrime.DESCRICAO_TIPO_CRIME;
                    denObj.outro_tipo_crime = den.OUTRO_TIPO_CRIME;
                    denObj.relato = den.RELATO;
                    denObj.id_status_pc = den.Situacao.ID_SITUACAO;
                    denObj.status_pc = den.Situacao.DESCRICAO_SITUACAO;
                    denObj.id_status_pm = den.Situacao1.ID_SITUACAO;
                    denObj.status_pm = den.Situacao1.DESCRICAO_SITUACAO;
                    denObj.just_descarte_pc = den.JUSTIFICATIVA_DESCARTE_PC;
                    denObj.just_reativo_pc = den.JUSTIFICATIVA_REATIVO_PC;
                    denObj.just_retorno_pc = den.JUSTIFICATIVA_RETORNO_PC;
                    denObj.just_descarte_pm = den.JUSTIFICATIVA_DESCARTE_PM;
                    denObj.just_reativo_pm = den.JUSTIFICATIVA_REATIVO_PM;
                    denObj.just_retorno_pm = den.JUSTIFICATIVA_RETORNO_PM;
                    denObj.resposta_pc = den.RESPOSTA_PC;
                    denObj.resposta_final_pc = den.RESPOSTA_FINAL_PC;
                    denObj.resposta_pm = den.RESPOSTA_PM;
                    denObj.resposta_final_pm = den.RESPOSTA_FINAL_PM;
                    denObj.tipo_endereco = den.TipoEndereco.DESCRICAO_TIPO_ENDERECO;
                    denObj.cep = den.LOGRADOURO_CEP;
                    denObj.endereco = den.LOGRADOURO;
                    denObj.endereco_numero = den.LOGRADOURO_NUMERO;
                    denObj.complemento = den.LOGRADOURO_COMPLEMENTO;
                    denObj.bairro = den.LOGRADOURO_BAIRRO;
                    denObj.cidade = den.CIDADE;
                    denObj.estado = den.ESTADO;
                    denObj.endereco_referencia = den.PONTO_REFERENCIA;
                    denObj.id_orgao_pc = den.Orgao != null ? den.Orgao.ID_ORGAO : 0;
                    denObj.id_orgao_pm = den.Orgao1 != null ? den.Orgao1.ID_ORGAO : 0;
                    denObj.orgao_pc = den.Orgao != null ? den.Orgao.NOME_ORGAO : string.Empty;
                    denObj.orgao_pm = den.Orgao1 != null ? den.Orgao1.NOME_ORGAO : string.Empty;
                    denObj.usuario_atendimento_pc = den.Usuario6 != null ? den.Usuario6.NOME_COMPLETO : string.Empty;
                    denObj.usuario_atendimento_pm = den.Usuario7 != null ? den.Usuario7.NOME_COMPLETO : string.Empty;
                    denObj.usuario_descarte_pc = den.Usuario8 != null ? den.Usuario8.NOME_COMPLETO : string.Empty;
                    denObj.usuario_descarte_pm = den.Usuario9 != null ? den.Usuario9.NOME_COMPLETO : string.Empty;
                    denObj.usuario_reativo_pc = den.Usuario10 != null ? den.Usuario10.NOME_COMPLETO : string.Empty;
                    denObj.usuario_reativo_pm = den.Usuario11 != null ? den.Usuario11.NOME_COMPLETO : string.Empty;
                    denObj.usuario_resposta_pc = den.Usuario12 != null ? den.Usuario12.NOME_COMPLETO : string.Empty;
                    denObj.usuario_resposta_pm = den.Usuario13 != null ? den.Usuario13.NOME_COMPLETO : string.Empty;
                    denObj.usuario_validacao_pc = den.Usuario14 != null ? den.Usuario14.NOME_COMPLETO : string.Empty;
                    denObj.usuario_validacao_pm = den.Usuario15 != null ? den.Usuario15.NOME_COMPLETO : string.Empty;
                    denObj.usuario_validacao_pc = den.Usuario16 != null ? den.Usuario16.NOME_COMPLETO : string.Empty;
                    denObj.usuario_validacao_pm = den.Usuario17 != null ? den.Usuario17.NOME_COMPLETO : string.Empty;
                    denObj.delegacia_elaboracao_pc = den.OrgaoElaboracaoPC != null ? den.OrgaoElaboracaoPC.NOME_ORGAO : string.Empty;
                    denObj.tipo_boletim_pc = den.TipoBoletimPC != null ? den.TipoBoletimPC.DESCRICAO_TIPO_BOLETIM : string.Empty;
                    denObj.numero_boletim_pc = den.NUMERO_BOLETIM_PC;
                    denObj.flag_resposta_pc = den.FLAG_RESPOSTA_POSITIVA_PC.HasValue ? (den.FLAG_RESPOSTA_POSITIVA_PC.Value ? "Sim" : "Não") : string.Empty;
                    denObj.flag_registroBO_pc = den.ID_DELEGACIA_ELABORACAO_PC.HasValue ? "Sim" : "Não";
                    denObj.delegacia_elaboracao_pm = den.OrgaoElaboracaoPM != null ? den.OrgaoElaboracaoPM.NOME_ORGAO : string.Empty;
                    denObj.tipo_boletim_pm = den.TipoBoletimPM != null ? den.TipoBoletimPM.DESCRICAO_TIPO_BOLETIM : string.Empty;
                    denObj.numero_boletim_pm = den.NUMERO_BOLETIM_PM;
                    denObj.flag_resposta_pm = den.FLAG_RESPOSTA_POSITIVA_PM.HasValue ? (den.FLAG_RESPOSTA_POSITIVA_PM.Value ? "Sim" : "Não") : string.Empty;
                    denObj.flag_registroBO_pm = den.ID_DELEGACIA_ELABORACAO_PM.HasValue ? "Sim" : "Não";
                    denObj.flag_boletim = den.FLAG_BOLETIM.HasValue ? (den.FLAG_BOLETIM.Value ? "Sim" : "Não") : "Não sei";
                    denObj.flag_porte_arma = den.FLAG_PORTE_ARMA_FOGO.HasValue ? (den.FLAG_PORTE_ARMA_FOGO.Value ? "Sim" : "Não") : "Não sei";
                    denObj.flag_local_fuga = den.LocalFuga != null ? den.LocalFuga.ID_LOCAL_FUGA == 5 : false;
                    denObj.local_fuga = den.LocalFuga != null ? den.LocalFuga.DESCRICAO_LOCAL_FUGA : string.Empty;
                    denObj.descricao_local_fuga = den.DESCRICAO_LOCAL_FUGA;

                    // Dados do denunciante e denunciados (Informações, Endereço, Telefone e Características)
                    if (den.DenunciaPessoa.Count > 0)
                    {
                        DenunciaPessoa denunciante = den.DenunciaPessoa.FirstOrDefault(dp => dp.ID_TIPO_PESSOA == 1);

                        if (!denunciante.FLAG_SIGILO.Value || atendimento)
                        {
                            PessoaTelefone resDenunciante = null;
                            PessoaTelefone celDenunciante = null;
                            PessoaEndereco endDenunciante = null;

                            if (denunciante.PessoaTelefone.Count > 0)
                            {
                                resDenunciante = denunciante.PessoaTelefone.FirstOrDefault(t => t.ID_TIPO_TELEFONE == 1 && t.ID_PESSOA == denunciante.ID_PESSOA);
                                celDenunciante = denunciante.PessoaTelefone.FirstOrDefault(t => t.ID_TIPO_TELEFONE == 3 && t.ID_PESSOA == denunciante.ID_PESSOA);
                            }

                            if (denunciante.PessoaEndereco.Count > 0)
                                endDenunciante = denunciante.PessoaEndereco.FirstOrDefault(e => e.ID_PESSOA == denunciante.ID_PESSOA);

                            denObj.denunciante = new DenuncianteModel()
                            {
                                nome = denunciante.NOME_PESSOA,
                                sexo = denunciante.Sexo != null ? denunciante.Sexo.DESCRICAO_SEXO : string.Empty,
                                tel_residencial = resDenunciante != null ? resDenunciante.PESSOA_TELEFONE : string.Empty,
                                tel_celular = celDenunciante != null ? celDenunciante.PESSOA_TELEFONE : string.Empty,
                                email = denunciante.EMAIL,
                                CPF = denunciante.CPF,
                                RG = denunciante.RG,
                                RG_UF = denunciante.RG_UF,
                                naturalidade = denunciante.NATURALIDADE,
                                data_nascimento = denunciante.DATA_NASCIMENTO.HasValue ? denunciante.DATA_NASCIMENTO.Value.ToString("dd/MM/yyyy") : string.Empty,
                                estado_civil = denunciante.EstadoCivil != null ? denunciante.EstadoCivil.DESCRICAO_ESTADO_CIVIL : string.Empty,
                                profissao = denunciante.PROFISSAO,
                                sigilo = denunciante.FLAG_SIGILO.HasValue ? (denunciante.FLAG_SIGILO.Value ? "Sim" : "Não") : string.Empty,
                                CEP = endDenunciante.LOGRADOURO_CEP,
                                endereco = endDenunciante.LOGRADOURO,
                                numero = endDenunciante.LOGRADOURO_NUMERO,
                                complemento = endDenunciante.LOGRADOURO_COMPLEMENTO,
                                bairro = endDenunciante.LOGRADOURO_BAIRRO,
                                cidade = endDenunciante.CIDADE,
                                estado = endDenunciante.ESTADO,
                                referencia = endDenunciante.PONTO_REFERENCIA,
                            };
                        }

                        denObj.denunciados = new List<DenunciadoModel>();
                        List<DenunciaPessoa> denunciados = den.DenunciaPessoa.Where(dp => dp.ID_TIPO_PESSOA == 2).ToList();

                        foreach (DenunciaPessoa dp in denunciados)
                        {
                            PessoaCaracteristicas carac = null;
                            PessoaEndereco endDenunciado = null;

                            if (dp.PessoaCaracteristicas.Count > 0)
                                carac = dp.PessoaCaracteristicas.FirstOrDefault(pc => pc.ID_PESSOA == dp.ID_PESSOA);

                            if (dp.PessoaEndereco.Count > 0)
                                endDenunciado = dp.PessoaEndereco.FirstOrDefault(ed => ed.ID_PESSOA == dp.ID_PESSOA);

                            denObj.denunciados.Add(
                                new DenunciadoModel()
                                {
                                    nome = dp.NOME_PESSOA,
                                    apelido = dp.APELIDO_PESSOA,
                                    sexo = carac.Sexo != null ? carac.Sexo.DESCRICAO_SEXO : string.Empty,
                                    idade = carac.FaixaEtaria != null ? carac.FaixaEtaria.DESCRICAO_FAIXA_ETARIA : string.Empty,
                                    estatura = carac.Estatura != null ? carac.Estatura.DESCRICAO_ESTATURA : string.Empty,
                                    tipo_fisico = carac.TipoFisico != null ? carac.TipoFisico.DESCRICAO_TIPO_FISICO : string.Empty,
                                    cor_pele = carac.CorPele != null ? carac.CorPele.DESCRICAO_COR : string.Empty,
                                    cor_olhos = carac.CorOlhos != null ? carac.CorOlhos.DESCRICAO_COR : string.Empty,
                                    flag_tipo_cabelo = carac.TipoCabelo != null ? carac.TipoCabelo.ID_TIPO_CABELO == 9 : false,
                                    tipo_cabelo = carac.TipoCabelo != null ? carac.TipoCabelo.DESCRICAO_TIPO_CABELO : string.Empty,
                                    desc_tipo_cabelo = carac.TipoCabelo != null ? (carac.TipoCabelo.ID_TIPO_CABELO == 9 ? carac.DESCRICAO_TIPO_CABELO : carac.TipoCabelo.DESCRICAO_TIPO_CABELO) : string.Empty,
                                    flag_cor_cabelo = carac.CorCabelo != null ? carac.CorCabelo.ID_COR_CABELO == 7 : false,
                                    cor_cabelo = carac.CorCabelo != null ? carac.CorCabelo.DESCRICAO_COR : string.Empty,
                                    desc_cor_cabelo = carac.CorCabelo != null ? (carac.CorCabelo.ID_COR_CABELO == 7 ? carac.DESCRICAO_COR_CABELO : carac.CorCabelo.DESCRICAO_COR) : string.Empty,
                                    flag_cicatriz = carac.Cicatriz != null ? (carac.Cicatriz.ID_CICATRIZ == 1) : false,
                                    cicatriz = carac.Cicatriz != null ? carac.Cicatriz.DESCRICAO_CICATRIZ : string.Empty,
                                    desc_cicatriz = carac.Cicatriz != null ? (carac.Cicatriz.ID_CICATRIZ == 1 ? carac.DESCRICAO_CICATRIZ : string.Empty) : string.Empty,
                                    flag_tatuagem = carac.Tatuagem != null ? (carac.Tatuagem.ID_TATUAGEM == 1) : false,
                                    tatuagem = carac.Tatuagem != null ? carac.Tatuagem.DESCRICAO_TATUAGEM : string.Empty,
                                    desc_tatuagem = carac.Tatuagem != null ? (carac.Tatuagem.ID_TATUAGEM == 1 ? carac.DESCRICAO_TATUAGEM : string.Empty) : string.Empty,
                                    flag_endereco = endDenunciado != null,
                                    tipo_endereco = endDenunciado == null ? string.Empty : endDenunciado.TipoEndereco.DESCRICAO_TIPO_ENDERECO,
                                    CEP = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO_CEP,
                                    endereco = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO,
                                    numero = endDenunciado == null ? 0 : endDenunciado.LOGRADOURO_NUMERO,
                                    complemento = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO_COMPLEMENTO,
                                    bairro = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO_BAIRRO,
                                    cidade = endDenunciado == null ? string.Empty : endDenunciado.CIDADE,
                                    estado = endDenunciado == null ? string.Empty : endDenunciado.ESTADO,
                                    referencia = endDenunciado == null ? string.Empty : endDenunciado.PONTO_REFERENCIA,
                                }
                            );
                        }
                    }

                    // Dados dos animais
                    if (den.DenunciaAnimais.Count > 0)
                    {
                        denObj.animais = new List<AnimalModel>();
                        den.DenunciaAnimais.ToList().ForEach(da =>
                            denObj.animais.Add(new AnimalModel()
                            {
                                tipo_animal = da.TipoAnimal.ID_TIPO_ANIMAL == 7 ? string.Format("{0} - {1}", da.TipoAnimal.DESCRICAO_TIPO_ANIMAL, da.OUTRO_TIPO_ANIMAL) : da.TipoAnimal.DESCRICAO_TIPO_ANIMAL,
                                porte_animal = da.PorteAnimal.DESCRICAO_PORTE_ANIMAL,
                                quantidade = da.QUANTIDADE,
                            })
                        );
                    }

                    // Dados dos veículos
                    if (den.DenunciaVeiculo.Count > 0)
                    {
                        denObj.veiculos = new List<VeiculoModel>();
                        den.DenunciaVeiculo.ToList().ForEach(dv =>
                            denObj.veiculos.Add(new VeiculoModel()
                            {
                                tipo_veiculo = dv.TipoVeiculo.DESCRICAO_TIPO_VEICULO,
                                placa = dv.NUMERO_PLACA,
                                marca = dv.MARCA_VEICULO,
                                cor_veiculo = dv.CorVeiculo.DESCRICAO_COR,
                                modelo = dv.MODELO_VEICULO,
                                observacoes = dv.OBSERVACOES,
                            })
                        );
                    }

                    // Dados dos anexos
                    if (den.DenunciaAnexo.Count > 0)
                    {
                        denObj.anexos = new List<AnexoModel>();
                        den.DenunciaAnexo.ToList().ForEach(da =>
                            denObj.anexos.Add(new AnexoModel()
                            {
                                endereco = da.ENDERECO_ANEXO,
                                nome = da.ID_ANEXO.ToString(),
                                original = da.NOME_ORIGINAL,
                                extensao = da.MIME_TYPE,
                                observacoes = da.OBSERVACOES,
                                preview = (new List<string>() { ".bmp", ".gif", ".jpe", ".jpeg", ".jpg", ".png", ".tif" }.Any(t => t == da.MIME_TYPE)),
                            })
                        );
                    }

                    // Dados das páginas de Internet
                    if (den.DenunciaLink.Count > 0)
                    {
                        denObj.links = new List<LinkModel>();
                        den.DenunciaLink.ToList().ForEach(dl =>
                            denObj.links.Add(new LinkModel()
                            {
                                endereco = dl.ENDERECO_LINK,
                                observacoes = dl.OBSERVACOES,
                            })
                        );
                    }

                    // Histórico de respostas
                    if (den.DenunciaRespostas.Count > 0)
                    {
                        List<DenunciaRespostas> civil = den.DenunciaRespostas.Where(r => r.FLAG_CIVIL).ToList();
                        List<DenunciaRespostas> militar = den.DenunciaRespostas.Where(r => r.FLAG_MILITAR).ToList();

                        denObj.historico_pc = new List<RespostaModel>();
                        civil.ForEach(rc => denObj.historico_pc.Add(new RespostaModel()
                        {
                            resposta = rc.RESPOSTA,
                            usuario_resposta = rc.UsuarioResposta.NOME_COMPLETO,
                            just_retorno = rc.JUSTIFICATIVA_RETORNO,
                            usuario_retorno = rc.UsuarioRetorno != null ? rc.UsuarioRetorno.NOME_COMPLETO : string.Empty,
                            data_resposta = rc.DATA_RESPOSTA.ToString("dd/MM/yyyy HH:mm:ss"),
                            data_retorno = rc.DATA_RETORNO.HasValue ? rc.DATA_RETORNO.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        })
                        );

                        denObj.historico_pm = new List<RespostaModel>();
                        militar.ForEach(rm => denObj.historico_pm.Add(new RespostaModel()
                        {
                            resposta = rm.RESPOSTA,
                            usuario_resposta = rm.UsuarioResposta.NOME_COMPLETO,
                            just_retorno = rm.JUSTIFICATIVA_RETORNO,
                            usuario_retorno = rm.UsuarioRetorno != null ? rm.UsuarioRetorno.NOME_COMPLETO : string.Empty,
                            data_resposta = rm.DATA_RESPOSTA.ToString("dd/MM/yyyy HH:mm:ss"),
                            data_retorno = rm.DATA_RETORNO.HasValue ? rm.DATA_RETORNO.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        })
                        );
                    }

                }

                return Ok(denObj);
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("Um erro ocorreu ao carregar os dados completos da denúncia:")
                .AppendLine(ex.ToString());
                LogErrorModel log = new Models.LogErrorModel(Request, error.ToString());

                PreenchimentoLog exc = new PreenchimentoLog()
                {
                    DATA_LOG = log.data,
                    NAVEGADOR = log.browser,
                    IP_CADASTRO = log.ip,
                    DESCRICAO_LOG = log.log,
                };
                _dbContext.PreenchimentoLog.Add(exc);
                _dbContext.SaveChanges();

                ModelState.AddModelError("", ex);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        // Abre as informações da denúncia de acordo com o perfil do usuário
        public IHttpActionResult abrirDenuncia(int ano, int numero, bool civil, bool militar)
        {
            try
            {
                DenunciaViewModel denObj = new DenunciaViewModel();

                Denuncia denuncia = _dbContext.Denuncia
                    .Include("DenunciaAnexo") // Lista de anexos
                    .Include("DenunciaAnimais") // Lista de animais
                    .Include("DenunciaLink") // Lista de endereços eletrônicos
                    .Include("DenunciaPessoa") // Lista de pessoas
                    .Include("DenunciaVeiculo") // Lista de veículos
                    .Include("Periodo") // Período
                    .Include("TipoCrime") // Tipo de crime
                    .Include("Situacao") // Situação PC
                    .Include("Situacao1") // Situação PM
                    .Include("TipoEndereco") // Tipo de endereço da ocorrência
                    .Include("LocalFuga") // Possível local de fuga marcado na denúncia
                    .Include("Orgao") // Orgão de encaminhamento PC
                    .Include("Orgao1") // Orgão de encamminhamento PM
                    .Include("OrgaoElaboracaoPC") // Delegacia que elaborou o BO (lado PC)
                    .Include("OrgaoElaboracaoPM") // Delegacia que elaborou o BO (lado PM)
                    .Include("TipoBoletimPC") // Tipo de Boletim PC
                    .Include("TipoBoletimPM") // Tipo de Boletim PM
                    .Include("Usuario6") // Usuário Atendimento PC
                    .Include("Usuario7") // Usuário Atendimento PM
                    .Include("Usuario8") // Usuário Descarte PC
                    .Include("Usuario9") // Usuário Descarte PM
                    .Include("Usuario10") // Usuário Reativação PC
                    .Include("Usuario11") // Usuário Reativação PM
                    .Include("Usuario12") // Usuário Resposta PC
                    .Include("Usuario13") // Usuário Resposta PM
                    .Include("Usuario14") // Usuário Validação PC
                    .Include("Usuario15") // Usuário Validação PM
                    .Include("Usuario16") // Usuário Retorno PC
                    .Include("Usuario17") // Usuário Retorno PM
                    .FirstOrDefault(d => d.ANO_DENUNCIA == ano && d.NUM_DENUNCIA == numero); // Recupera a denúncia pela chave principal

                if (denuncia != null)
                {
                    if (civil)
                        denObj = abrirDenunciaPC(denuncia);

                    if (militar)
                        denObj = abrirDenunciaPM(denuncia);
                }

                return Ok(denObj);
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("Um erro ocorreu ao carregar os dados da denúncia:")
                .AppendLine(ex.ToString());
                LogErrorModel log = new Models.LogErrorModel(Request, error.ToString());

                PreenchimentoLog exc = new PreenchimentoLog()
                {
                    DATA_LOG = log.data,
                    NAVEGADOR = log.browser,
                    IP_CADASTRO = log.ip,
                    DESCRICAO_LOG = log.log,
                };
                _dbContext.PreenchimentoLog.Add(exc);
                _dbContext.SaveChanges();

                ModelState.AddModelError("", ex);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        // Retorna a lista de situações existentes no sistema
        public IHttpActionResult listarSituacao()
        {
            try
            {
                return Ok(
                    _dbContext.Situacao.AsNoTracking().Where(s => s.FLAG_ATIVO)
                    .Select(s => new SituacaoModel() { ID_SITUACAO = s.ID_SITUACAO, DESCRICAO_SITUACAO = s.DESCRICAO_SITUACAO }));
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("Um erro ocorreu ao listar as situações:")
                .AppendLine(ex.ToString());
                LogErrorModel log = new Models.LogErrorModel(Request, error.ToString());

                PreenchimentoLog exc = new PreenchimentoLog()
                {
                    DATA_LOG = log.data,
                    NAVEGADOR = log.browser,
                    IP_CADASTRO = log.ip,
                    DESCRICAO_LOG = log.log,
                };
                _dbContext.PreenchimentoLog.Add(exc);
                _dbContext.SaveChanges();

                ModelState.AddModelError("", ex);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        // Retorna a lista de tipo de crime existentes no sistema
        public IHttpActionResult listarTipoCrime()
        {
            try
            {
                return Ok(
                    _dbContext.TipoCrime.AsNoTracking().Where(t => t.FLAG_ATIVO)
                    .Select(t => new CrimeModel() { ID_TIPO_CRIME = t.ID_TIPO_CRIME, DESCRICAO_TIPO_CRIME = t.DESCRICAO_TIPO_CRIME }));
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("Um erro ocorreu ao listar os tipos de crimes:")
                .AppendLine(ex.ToString());
                LogErrorModel log = new Models.LogErrorModel(Request, error.ToString());

                PreenchimentoLog exc = new PreenchimentoLog()
                {
                    DATA_LOG = log.data,
                    NAVEGADOR = log.browser,
                    IP_CADASTRO = log.ip,
                    DESCRICAO_LOG = log.log,
                };
                _dbContext.PreenchimentoLog.Add(exc);
                _dbContext.SaveChanges();

                ModelState.AddModelError("", ex);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        // Retorna a lista de delegacias existentes no sistema
        public IHttpActionResult listarDelegacia()
        {
            try
            {
                return 
                    Ok(_dbContext.Orgao.AsNoTracking().Where(o =>
                    o.ID_TIPO_ORGAO == 2 && o.ID_SUBTIPO_ORGAO == 4 && o.FLAG_ATIVO)
                    .Select(o => new OrgaoModel() { ID_ORGAO = o.ID_ORGAO, NOME_ORGAO = o.NOME_ORGAO }));
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("Um erro ocorreu ao listar as delegacias:")
                .AppendLine(ex.ToString());
                LogErrorModel log = new Models.LogErrorModel(Request, error.ToString());

                PreenchimentoLog exc = new PreenchimentoLog()
                {
                    DATA_LOG = log.data,
                    NAVEGADOR = log.browser,
                    IP_CADASTRO = log.ip,
                    DESCRICAO_LOG = log.log,
                };
                _dbContext.PreenchimentoLog.Add(exc);
                _dbContext.SaveChanges();

                ModelState.AddModelError("", ex);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        // Retorna a lista de batalhões existentes no sistema
        public IHttpActionResult listarBatalhao()
        {
            try
            {
                return Ok(
                    _dbContext.Orgao.AsNoTracking().Where(o =>
                    o.ID_TIPO_ORGAO == 3 && o.ID_SUBTIPO_ORGAO == 7 && o.FLAG_ATIVO)
                    .Select(o => new OrgaoModel() { ID_ORGAO = o.ID_ORGAO, NOME_ORGAO = o.NOME_ORGAO }));
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("Um erro ocorreu ao listar os batalhões:")
                .AppendLine(ex.ToString());
                LogErrorModel log = new Models.LogErrorModel(Request, error.ToString());

                PreenchimentoLog exc = new PreenchimentoLog()
                {
                    DATA_LOG = log.data,
                    NAVEGADOR = log.browser,
                    IP_CADASTRO = log.ip,
                    DESCRICAO_LOG = log.log,
                };
                _dbContext.PreenchimentoLog.Add(exc);
                _dbContext.SaveChanges();

                ModelState.AddModelError("", ex);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        // Retorna a lista de companias existentes no sistema
        public IHttpActionResult listarCompania()
        {
            try
            {
                return Ok(
                    _dbContext.Orgao.AsNoTracking().Where(o => 
                    o.ID_TIPO_ORGAO == 3 && o.ID_SUBTIPO_ORGAO == 8 && o.FLAG_ATIVO)
                    .Select(o => new OrgaoModel() { ID_ORGAO = o.ID_ORGAO, NOME_ORGAO = o.NOME_ORGAO }));
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("Um erro ocorreu ao listar as companias:")
                .AppendLine(ex.ToString());
                LogErrorModel log = new Models.LogErrorModel(Request, error.ToString());

                PreenchimentoLog exc = new PreenchimentoLog()
                {
                    DATA_LOG = log.data,
                    NAVEGADOR = log.browser,
                    IP_CADASTRO = log.ip,
                    DESCRICAO_LOG = log.log,
                };
                _dbContext.PreenchimentoLog.Add(exc);
                _dbContext.SaveChanges();

                ModelState.AddModelError("", ex);
                return BadRequest(ModelState);
            }
        }

        #endregion

        #region POST

        #endregion

        #region Methods

        public DenunciaViewModel abrirDenunciaPC(Denuncia den)
        {
            DenunciaViewModel retorno = new DenunciaViewModel();

            // Dados da denúncia
            retorno.ano = den.ANO_DENUNCIA;
            retorno.numero = den.NUM_DENUNCIA;
            retorno.periodo = den.Periodo.DESCRICAO_PERIODO;
            retorno.data_cadastro = den.DATA_CADASTRO.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.data_ocorrencia = den.DATA_OCORRENCIA.ToString("dd/MM/yyyy");
            retorno.data_atendimento = den.DATA_ATENDIMENTO_PC.HasValue ? den.DATA_ATENDIMENTO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_encaminhamento = den.DATA_ENCAMINHAMENTO_PC.HasValue ? den.DATA_ENCAMINHAMENTO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_resposta = den.DATA_RESPOSTA_PC.HasValue ? den.DATA_RESPOSTA_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_validacao = den.DATA_VALIDACAO_PC.HasValue ? den.DATA_VALIDACAO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_descarte = den.DATA_DESCARTE_PC.HasValue ? den.DATA_DESCARTE_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_reativo = den.DATA_REATIVO_PC.HasValue ? den.DATA_REATIVO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_retorno = den.DATA_RETORNO_PC.HasValue ? den.DATA_RETORNO_PC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_providencia = den.DATA_PROVIDENCIA_PC.HasValue ? den.DATA_PROVIDENCIA_PC.Value.ToString("dd/MM/yyyy") : string.Empty;
            retorno.flag_tipo_crime = den.TipoCrime.ID_TIPO_CRIME == 6;
            retorno.tipo_crime = den.TipoCrime.DESCRICAO_TIPO_CRIME;
            retorno.outro_tipo_crime = den.OUTRO_TIPO_CRIME;
            retorno.status = den.Situacao.DESCRICAO_SITUACAO;
            retorno.relato = den.RELATO;
            retorno.just_descarte = den.JUSTIFICATIVA_DESCARTE_PC;
            retorno.just_reativo = den.JUSTIFICATIVA_REATIVO_PC;
            retorno.just_retorno = den.JUSTIFICATIVA_RETORNO_PC;
            retorno.resposta = den.RESPOSTA_PC;
            retorno.resposta_final = den.RESPOSTA_FINAL_PC;
            retorno.tipo_endereco = den.TipoEndereco.DESCRICAO_TIPO_ENDERECO;
            retorno.cep = den.LOGRADOURO_CEP;
            retorno.endereco = den.LOGRADOURO;
            retorno.endereco_numero = den.LOGRADOURO_NUMERO;
            retorno.complemento = den.LOGRADOURO_COMPLEMENTO;
            retorno.bairro = den.LOGRADOURO_BAIRRO;
            retorno.cidade = den.CIDADE;
            retorno.estado = den.ESTADO;
            retorno.endereco_referencia = den.PONTO_REFERENCIA;
            retorno.id_orgao = den.Orgao != null ? den.Orgao.ID_ORGAO : 0;
            retorno.orgao = den.Orgao != null ? den.Orgao.NOME_ORGAO : string.Empty;
            retorno.usuario_atendimento = den.Usuario6 != null ? den.Usuario6.NOME_COMPLETO : string.Empty;
            retorno.usuario_descarte = den.Usuario8 != null ? den.Usuario8.NOME_COMPLETO : string.Empty;
            retorno.usuario_reativo = den.Usuario10 != null ? den.Usuario10.NOME_COMPLETO : string.Empty;
            retorno.usuario_resposta = den.Usuario12 != null ? den.Usuario12.NOME_COMPLETO : string.Empty;
            retorno.usuario_validacao = den.Usuario14 != null ? den.Usuario14.NOME_COMPLETO : string.Empty;
            retorno.usuario_retorno = den.Usuario16 != null ? den.Usuario16.NOME_COMPLETO : string.Empty;
            retorno.delegacia_elaboracao = den.OrgaoElaboracaoPC != null ? den.OrgaoElaboracaoPC.NOME_ORGAO : string.Empty;
            retorno.tipo_boletim = den.TipoBoletimPC != null ? den.TipoBoletimPC.DESCRICAO_TIPO_BOLETIM : string.Empty;
            retorno.numero_boletim = den.NUMERO_BOLETIM_PC;
            retorno.flag_resposta = den.FLAG_RESPOSTA_POSITIVA_PC.HasValue ? (den.FLAG_RESPOSTA_POSITIVA_PC.Value ? "Sim" : "Não") : string.Empty;
            retorno.flag_registroBO = den.ID_DELEGACIA_ELABORACAO_PC.HasValue ? "Sim" : "Não";
            retorno.flag_boletim = den.FLAG_BOLETIM.HasValue ? (den.FLAG_BOLETIM.Value ? "Sim" : "Não") : "Não sei";
            retorno.flag_porte_arma = den.FLAG_PORTE_ARMA_FOGO.HasValue ? (den.FLAG_PORTE_ARMA_FOGO.Value ? "Sim" : "Não") : "Não sei";
            retorno.flag_local_fuga = den.LocalFuga != null ? den.LocalFuga.ID_LOCAL_FUGA == 5 : false;
            retorno.local_fuga = den.LocalFuga != null ? den.LocalFuga.DESCRICAO_LOCAL_FUGA : string.Empty;
            retorno.descricao_local_fuga = den.DESCRICAO_LOCAL_FUGA;

            // Dados do denunciante e denunciados (Informações, Endereço, Telefone e Características)
            if (den.DenunciaPessoa.Count > 0)
            {
                DenunciaPessoa denunciante = den.DenunciaPessoa.FirstOrDefault(dp => dp.ID_TIPO_PESSOA == 1);

                PessoaTelefone resDenunciante = null;
                PessoaTelefone celDenunciante = null;
                PessoaEndereco endDenunciante = null;

                if (denunciante.PessoaTelefone.Count > 0)
                {
                    resDenunciante = denunciante.PessoaTelefone.FirstOrDefault(t => t.ID_TIPO_TELEFONE == 1 && t.ID_PESSOA == denunciante.ID_PESSOA);
                    celDenunciante = denunciante.PessoaTelefone.FirstOrDefault(t => t.ID_TIPO_TELEFONE == 3 && t.ID_PESSOA == denunciante.ID_PESSOA);
                }

                if (denunciante.PessoaEndereco.Count > 0)
                    endDenunciante = denunciante.PessoaEndereco.FirstOrDefault(e => e.ID_PESSOA == denunciante.ID_PESSOA);

                retorno.denunciante = new DenuncianteModel()
                {
                    nome = denunciante.NOME_PESSOA,
                    sexo = denunciante.Sexo != null ? denunciante.Sexo.DESCRICAO_SEXO : string.Empty,
                    tel_residencial = resDenunciante != null ? resDenunciante.PESSOA_TELEFONE : string.Empty,
                    tel_celular = celDenunciante != null ? celDenunciante.PESSOA_TELEFONE : string.Empty,
                    email = denunciante.EMAIL,
                    CPF = denunciante.CPF,
                    RG = denunciante.RG,
                    RG_UF = denunciante.RG_UF,
                    naturalidade = denunciante.NATURALIDADE,
                    data_nascimento = denunciante.DATA_NASCIMENTO.HasValue ? denunciante.DATA_NASCIMENTO.Value.ToString("dd/MM/yyyy") : string.Empty,
                    estado_civil = denunciante.EstadoCivil != null ? denunciante.EstadoCivil.DESCRICAO_ESTADO_CIVIL : string.Empty,
                    profissao = denunciante.PROFISSAO,
                    sigilo = denunciante.FLAG_SIGILO.HasValue ? (denunciante.FLAG_SIGILO.Value ? "Sim" : "Não") : string.Empty,
                    CEP = endDenunciante.LOGRADOURO_CEP,
                    endereco = endDenunciante.LOGRADOURO,
                    numero = endDenunciante.LOGRADOURO_NUMERO,
                    complemento = endDenunciante.LOGRADOURO_COMPLEMENTO,
                    bairro = endDenunciante.LOGRADOURO_BAIRRO,
                    cidade = endDenunciante.CIDADE,
                    estado = endDenunciante.ESTADO,
                    referencia = endDenunciante.PONTO_REFERENCIA,
                };

                retorno.denunciados = new List<DenunciadoModel>();
                List<DenunciaPessoa> denunciados = den.DenunciaPessoa.Where(dp => dp.ID_TIPO_PESSOA == 2).ToList();

                foreach (DenunciaPessoa dp in denunciados)
                {
                    PessoaCaracteristicas carac = null;
                    PessoaEndereco endDenunciado = null;

                    if (dp.PessoaCaracteristicas.Count > 0)
                        carac = dp.PessoaCaracteristicas.FirstOrDefault(pc => pc.ID_PESSOA == dp.ID_PESSOA);

                    if (dp.PessoaEndereco.Count > 0)
                        endDenunciado = dp.PessoaEndereco.FirstOrDefault(ed => ed.ID_PESSOA == dp.ID_PESSOA);

                    retorno.denunciados.Add(
                        new DenunciadoModel()
                        {
                            nome = dp.NOME_PESSOA,
                            apelido = dp.APELIDO_PESSOA,
                            sexo = carac.Sexo != null ? carac.Sexo.DESCRICAO_SEXO : string.Empty,
                            idade = carac.FaixaEtaria != null ? carac.FaixaEtaria.DESCRICAO_FAIXA_ETARIA : string.Empty,
                            estatura = carac.Estatura != null ? carac.Estatura.DESCRICAO_ESTATURA : string.Empty,
                            tipo_fisico = carac.TipoFisico != null ? carac.TipoFisico.DESCRICAO_TIPO_FISICO : string.Empty,
                            cor_pele = carac.CorPele != null ? carac.CorPele.DESCRICAO_COR : string.Empty,
                            cor_olhos = carac.CorOlhos != null ? carac.CorOlhos.DESCRICAO_COR : string.Empty,
                            flag_tipo_cabelo = carac.TipoCabelo != null ? carac.TipoCabelo.ID_TIPO_CABELO == 9 : false,
                            tipo_cabelo = carac.TipoCabelo != null ? carac.TipoCabelo.DESCRICAO_TIPO_CABELO : string.Empty,
                            desc_tipo_cabelo = carac.TipoCabelo != null ? (carac.TipoCabelo.ID_TIPO_CABELO == 9 ? carac.DESCRICAO_TIPO_CABELO : carac.TipoCabelo.DESCRICAO_TIPO_CABELO) : string.Empty,
                            flag_cor_cabelo = carac.CorCabelo != null ? carac.CorCabelo.ID_COR_CABELO == 7 : false,
                            cor_cabelo = carac.CorCabelo != null ? carac.CorCabelo.DESCRICAO_COR : string.Empty,
                            desc_cor_cabelo = carac.CorCabelo != null ? (carac.CorCabelo.ID_COR_CABELO == 7 ? carac.DESCRICAO_COR_CABELO : carac.CorCabelo.DESCRICAO_COR) : string.Empty,
                            flag_cicatriz = carac.Cicatriz != null ? (carac.Cicatriz.ID_CICATRIZ == 1) : false,
                            cicatriz = carac.Cicatriz != null ? carac.Cicatriz.DESCRICAO_CICATRIZ : string.Empty,
                            desc_cicatriz = carac.Cicatriz != null ? (carac.Cicatriz.ID_CICATRIZ == 1 ? carac.DESCRICAO_CICATRIZ : string.Empty) : string.Empty,
                            flag_tatuagem = carac.Tatuagem != null ? (carac.Tatuagem.ID_TATUAGEM == 1) : false,
                            tatuagem = carac.Tatuagem != null ? carac.Tatuagem.DESCRICAO_TATUAGEM : string.Empty,
                            desc_tatuagem = carac.Tatuagem != null ? (carac.Tatuagem.ID_TATUAGEM == 1 ? carac.DESCRICAO_TATUAGEM : string.Empty) : string.Empty,
                            flag_endereco = endDenunciado != null,
                            tipo_endereco = endDenunciado == null ? string.Empty : endDenunciado.TipoEndereco.DESCRICAO_TIPO_ENDERECO,
                            CEP = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO_CEP,
                            endereco = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO,
                            numero = endDenunciado == null ? 0 : endDenunciado.LOGRADOURO_NUMERO,
                            complemento = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO_COMPLEMENTO,
                            bairro = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO_BAIRRO,
                            cidade = endDenunciado == null ? string.Empty : endDenunciado.CIDADE,
                            estado = endDenunciado == null ? string.Empty : endDenunciado.ESTADO,
                            referencia = endDenunciado == null ? string.Empty : endDenunciado.PONTO_REFERENCIA,
                        }
                    );
                }
            }

            //Dados dos animais
            if (den.DenunciaAnimais.Count > 0)
            {
                retorno.animais = new List<AnimalModel>();
                den.DenunciaAnimais.ToList().ForEach(da =>
                    retorno.animais.Add(new AnimalModel()
                    {
                        tipo_animal = da.TipoAnimal.ID_TIPO_ANIMAL == 7 ? string.Format("{0} - {1}", da.TipoAnimal.DESCRICAO_TIPO_ANIMAL, da.OUTRO_TIPO_ANIMAL) : da.TipoAnimal.DESCRICAO_TIPO_ANIMAL,
                        porte_animal = da.PorteAnimal.DESCRICAO_PORTE_ANIMAL,
                        quantidade = da.QUANTIDADE,
                    })
                );
            }

            // Dados dos veículos
            if (den.DenunciaVeiculo.Count > 0)
            {
                retorno.veiculos = new List<VeiculoModel>();
                den.DenunciaVeiculo.ToList().ForEach(dv =>
                    retorno.veiculos.Add(new VeiculoModel()
                    {
                        tipo_veiculo = dv.TipoVeiculo.DESCRICAO_TIPO_VEICULO,
                        placa = dv.NUMERO_PLACA,
                        marca = dv.MARCA_VEICULO,
                        cor_veiculo = dv.CorVeiculo.DESCRICAO_COR,
                        modelo = dv.MODELO_VEICULO,
                        observacoes = dv.OBSERVACOES,
                    })
                );
            }

            // Dados dos anexos
            if (den.DenunciaAnexo.Count > 0)
            {
                retorno.anexos = new List<AnexoModel>();
                den.DenunciaAnexo.ToList().ForEach(da =>
                    retorno.anexos.Add(new AnexoModel()
                    {
                        endereco = da.ENDERECO_ANEXO,
                        nome = da.ID_ANEXO.ToString(),
                        original = da.NOME_ORIGINAL,
                        extensao = da.MIME_TYPE,
                        observacoes = da.OBSERVACOES,
                        preview = (new List<string>() { ".bmp", ".gif", ".jpe", ".jpeg", ".jpg", ".png", ".tif" }.Any(t => t == da.MIME_TYPE)),
                    })
                );
            }

            // Dados das páginas de Internet
            if (den.DenunciaLink.Count > 0)
            {
                retorno.links = new List<LinkModel>();
                den.DenunciaLink.ToList().ForEach(dl =>
                    retorno.links.Add(new LinkModel()
                    {
                        endereco = dl.ENDERECO_LINK,
                        observacoes = dl.OBSERVACOES,
                    })
                );
            }

            return retorno;
        }

        public DenunciaViewModel abrirDenunciaPM(Denuncia den)
        {
            DenunciaViewModel retorno = new DenunciaViewModel();

            // Dados da denúncia
            retorno.ano = den.ANO_DENUNCIA;
            retorno.numero = den.NUM_DENUNCIA;
            retorno.periodo = den.Periodo.DESCRICAO_PERIODO;
            retorno.data_cadastro = den.DATA_CADASTRO.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.data_ocorrencia = den.DATA_OCORRENCIA.ToString("dd/MM/yyyy");
            retorno.data_atendimento = den.DATA_ATENDIMENTO_PM.HasValue ? den.DATA_ATENDIMENTO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_encaminhamento = den.DATA_ENCAMINHAMENTO_PM.HasValue ? den.DATA_ENCAMINHAMENTO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_resposta = den.DATA_RESPOSTA_PM.HasValue ? den.DATA_RESPOSTA_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_validacao = den.DATA_VALIDACAO_PM.HasValue ? den.DATA_VALIDACAO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_descarte = den.DATA_DESCARTE_PM.HasValue ? den.DATA_DESCARTE_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_reativo = den.DATA_REATIVO_PM.HasValue ? den.DATA_REATIVO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_retorno = den.DATA_RETORNO_PM.HasValue ? den.DATA_RETORNO_PM.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            retorno.data_providencia = den.DATA_PROVIDENCIA_PM.HasValue ? den.DATA_PROVIDENCIA_PM.Value.ToString("dd/MM/yyyy") : string.Empty;
            retorno.flag_tipo_crime = den.TipoCrime.ID_TIPO_CRIME == 6;
            retorno.tipo_crime = den.TipoCrime.DESCRICAO_TIPO_CRIME;
            retorno.outro_tipo_crime = den.OUTRO_TIPO_CRIME;
            retorno.status = den.Situacao1.DESCRICAO_SITUACAO;
            retorno.relato = den.RELATO;
            retorno.just_descarte = den.JUSTIFICATIVA_DESCARTE_PM;
            retorno.just_reativo = den.JUSTIFICATIVA_REATIVO_PM;
            retorno.just_retorno = den.JUSTIFICATIVA_RETORNO_PM;
            retorno.resposta = den.RESPOSTA_PM;
            retorno.resposta_final = den.RESPOSTA_FINAL_PM;
            retorno.tipo_endereco = den.TipoEndereco.DESCRICAO_TIPO_ENDERECO;
            retorno.cep = den.LOGRADOURO_CEP;
            retorno.endereco = den.LOGRADOURO;
            retorno.endereco_numero = den.LOGRADOURO_NUMERO;
            retorno.complemento = den.LOGRADOURO_COMPLEMENTO;
            retorno.bairro = den.LOGRADOURO_BAIRRO;
            retorno.cidade = den.CIDADE;
            retorno.estado = den.ESTADO;
            retorno.endereco_referencia = den.PONTO_REFERENCIA;
            retorno.id_orgao = den.Orgao1 != null ? den.Orgao1.ID_ORGAO : 0;
            retorno.orgao = den.Orgao1 != null ? den.Orgao1.NOME_ORGAO : string.Empty;
            retorno.usuario_atendimento = den.Usuario7 != null ? den.Usuario7.NOME_COMPLETO : string.Empty;
            retorno.usuario_descarte = den.Usuario9 != null ? den.Usuario9.NOME_COMPLETO : string.Empty;
            retorno.usuario_reativo = den.Usuario11 != null ? den.Usuario11.NOME_COMPLETO : string.Empty;
            retorno.usuario_resposta = den.Usuario13 != null ? den.Usuario13.NOME_COMPLETO : string.Empty;
            retorno.usuario_validacao = den.Usuario15 != null ? den.Usuario15.NOME_COMPLETO : string.Empty;
            retorno.usuario_retorno = den.Usuario17 != null ? den.Usuario17.NOME_COMPLETO : string.Empty;
            retorno.delegacia_elaboracao = den.OrgaoElaboracaoPM != null ? den.OrgaoElaboracaoPM.NOME_ORGAO : string.Empty;
            retorno.tipo_boletim = den.TipoBoletimPM != null ? den.TipoBoletimPM.DESCRICAO_TIPO_BOLETIM : string.Empty;
            retorno.numero_boletim = den.NUMERO_BOLETIM_PM;
            retorno.flag_resposta = den.FLAG_RESPOSTA_POSITIVA_PM.HasValue ? (den.FLAG_RESPOSTA_POSITIVA_PM.Value ? "Sim" : "Não") : string.Empty;
            retorno.flag_registroBO = den.ID_DELEGACIA_ELABORACAO_PM.HasValue ? "Sim" : "Não";
            retorno.flag_boletim = den.FLAG_BOLETIM.HasValue ? (den.FLAG_BOLETIM.Value ? "Sim" : "Não") : "Não sei";
            retorno.flag_porte_arma = den.FLAG_PORTE_ARMA_FOGO.HasValue ? (den.FLAG_PORTE_ARMA_FOGO.Value ? "Sim" : "Não") : "Não sei";
            retorno.flag_local_fuga = den.LocalFuga != null ? den.LocalFuga.ID_LOCAL_FUGA == 5 : false;
            retorno.local_fuga = den.LocalFuga != null ? den.LocalFuga.DESCRICAO_LOCAL_FUGA : string.Empty;
            retorno.descricao_local_fuga = den.DESCRICAO_LOCAL_FUGA;

            // Dados do denunciante e denunciados (Informações, Endereço, Telefone e Características)
            if (den.DenunciaPessoa.Count > 0)
            {
                DenunciaPessoa denunciante = den.DenunciaPessoa.FirstOrDefault(dp => dp.ID_TIPO_PESSOA == 1);

                PessoaTelefone resDenunciante = null;
                PessoaTelefone celDenunciante = null;
                PessoaEndereco endDenunciante = null;

                if (denunciante.PessoaTelefone.Count > 0)
                {
                    resDenunciante = denunciante.PessoaTelefone.FirstOrDefault(t => t.ID_TIPO_TELEFONE == 1 && t.ID_PESSOA == denunciante.ID_PESSOA);
                    celDenunciante = denunciante.PessoaTelefone.FirstOrDefault(t => t.ID_TIPO_TELEFONE == 3 && t.ID_PESSOA == denunciante.ID_PESSOA);
                }

                if (denunciante.PessoaEndereco.Count > 0)
                    endDenunciante = denunciante.PessoaEndereco.FirstOrDefault(e => e.ID_PESSOA == denunciante.ID_PESSOA);

                retorno.denunciante = new DenuncianteModel()
                {
                    nome = denunciante.NOME_PESSOA,
                    sexo = denunciante.Sexo != null ? denunciante.Sexo.DESCRICAO_SEXO : string.Empty,
                    tel_residencial = resDenunciante != null ? resDenunciante.PESSOA_TELEFONE : string.Empty,
                    tel_celular = celDenunciante != null ? celDenunciante.PESSOA_TELEFONE : string.Empty,
                    email = denunciante.EMAIL,
                    CPF = denunciante.CPF,
                    RG = denunciante.RG,
                    RG_UF = denunciante.RG_UF,
                    naturalidade = denunciante.NATURALIDADE,
                    data_nascimento = denunciante.DATA_NASCIMENTO.HasValue ? denunciante.DATA_NASCIMENTO.Value.ToString("dd/MM/yyyy") : string.Empty,
                    estado_civil = denunciante.EstadoCivil != null ? denunciante.EstadoCivil.DESCRICAO_ESTADO_CIVIL : string.Empty,
                    profissao = denunciante.PROFISSAO,
                    sigilo = denunciante.FLAG_SIGILO.HasValue ? (denunciante.FLAG_SIGILO.Value ? "Sim" : "Não") : string.Empty,
                    CEP = endDenunciante.LOGRADOURO_CEP,
                    endereco = endDenunciante.LOGRADOURO,
                    numero = endDenunciante.LOGRADOURO_NUMERO,
                    complemento = endDenunciante.LOGRADOURO_COMPLEMENTO,
                    bairro = endDenunciante.LOGRADOURO_BAIRRO,
                    cidade = endDenunciante.CIDADE,
                    estado = endDenunciante.ESTADO,
                    referencia = endDenunciante.PONTO_REFERENCIA,
                };

                retorno.denunciados = new List<DenunciadoModel>();
                List<DenunciaPessoa> denunciados = den.DenunciaPessoa.Where(dp => dp.ID_TIPO_PESSOA == 2).ToList();

                foreach (DenunciaPessoa dp in denunciados)
                {
                    PessoaCaracteristicas carac = null;
                    PessoaEndereco endDenunciado = null;

                    if (dp.PessoaCaracteristicas.Count > 0)
                        carac = dp.PessoaCaracteristicas.FirstOrDefault(pc => pc.ID_PESSOA == dp.ID_PESSOA);

                    if (dp.PessoaEndereco.Count > 0)
                        endDenunciado = dp.PessoaEndereco.FirstOrDefault(ed => ed.ID_PESSOA == dp.ID_PESSOA);

                    retorno.denunciados.Add(
                        new DenunciadoModel()
                        {
                            nome = dp.NOME_PESSOA,
                            apelido = dp.APELIDO_PESSOA,
                            sexo = carac.Sexo != null ? carac.Sexo.DESCRICAO_SEXO : string.Empty,
                            idade = carac.FaixaEtaria != null ? carac.FaixaEtaria.DESCRICAO_FAIXA_ETARIA : string.Empty,
                            estatura = carac.Estatura != null ? carac.Estatura.DESCRICAO_ESTATURA : string.Empty,
                            tipo_fisico = carac.TipoFisico != null ? carac.TipoFisico.DESCRICAO_TIPO_FISICO : string.Empty,
                            cor_pele = carac.CorPele != null ? carac.CorPele.DESCRICAO_COR : string.Empty,
                            cor_olhos = carac.CorOlhos != null ? carac.CorOlhos.DESCRICAO_COR : string.Empty,
                            flag_tipo_cabelo = carac.TipoCabelo != null ? carac.TipoCabelo.ID_TIPO_CABELO == 9 : false,
                            tipo_cabelo = carac.TipoCabelo != null ? carac.TipoCabelo.DESCRICAO_TIPO_CABELO : string.Empty,
                            desc_tipo_cabelo = carac.TipoCabelo != null ? (carac.TipoCabelo.ID_TIPO_CABELO == 9 ? carac.DESCRICAO_TIPO_CABELO : carac.TipoCabelo.DESCRICAO_TIPO_CABELO) : string.Empty,
                            flag_cor_cabelo = carac.CorCabelo != null ? carac.CorCabelo.ID_COR_CABELO == 7 : false,
                            cor_cabelo = carac.CorCabelo != null ? carac.CorCabelo.DESCRICAO_COR : string.Empty,
                            desc_cor_cabelo = carac.CorCabelo != null ? (carac.CorCabelo.ID_COR_CABELO == 7 ? carac.DESCRICAO_COR_CABELO : carac.CorCabelo.DESCRICAO_COR) : string.Empty,
                            flag_cicatriz = carac.Cicatriz != null ? (carac.Cicatriz.ID_CICATRIZ == 1) : false,
                            cicatriz = carac.Cicatriz != null ? carac.Cicatriz.DESCRICAO_CICATRIZ : string.Empty,
                            desc_cicatriz = carac.Cicatriz != null ? (carac.Cicatriz.ID_CICATRIZ == 1 ? carac.DESCRICAO_CICATRIZ : string.Empty) : string.Empty,
                            flag_tatuagem = carac.Tatuagem != null ? (carac.Tatuagem.ID_TATUAGEM == 1) : false,
                            tatuagem = carac.Tatuagem != null ? carac.Tatuagem.DESCRICAO_TATUAGEM : string.Empty,
                            desc_tatuagem = carac.Tatuagem != null ? (carac.Tatuagem.ID_TATUAGEM == 1 ? carac.DESCRICAO_TATUAGEM : string.Empty) : string.Empty,
                            flag_endereco = endDenunciado != null,
                            tipo_endereco = endDenunciado == null ? string.Empty : endDenunciado.TipoEndereco.DESCRICAO_TIPO_ENDERECO,
                            CEP = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO_CEP,
                            endereco = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO,
                            numero = endDenunciado == null ? 0 : endDenunciado.LOGRADOURO_NUMERO,
                            complemento = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO_COMPLEMENTO,
                            bairro = endDenunciado == null ? string.Empty : endDenunciado.LOGRADOURO_BAIRRO,
                            cidade = endDenunciado == null ? string.Empty : endDenunciado.CIDADE,
                            estado = endDenunciado == null ? string.Empty : endDenunciado.ESTADO,
                            referencia = endDenunciado == null ? string.Empty : endDenunciado.PONTO_REFERENCIA,
                        }
                    );
                }
            }

            //Dados dos animais
            if (den.DenunciaAnimais.Count > 0)
            {
                retorno.animais = new List<AnimalModel>();
                den.DenunciaAnimais.ToList().ForEach(da =>
                    retorno.animais.Add(new AnimalModel()
                    {
                        tipo_animal = da.TipoAnimal.ID_TIPO_ANIMAL == 7 ? string.Format("{0} - {1}", da.TipoAnimal.DESCRICAO_TIPO_ANIMAL, da.OUTRO_TIPO_ANIMAL) : da.TipoAnimal.DESCRICAO_TIPO_ANIMAL,
                        porte_animal = da.PorteAnimal.DESCRICAO_PORTE_ANIMAL,
                        quantidade = da.QUANTIDADE,
                    })
                );
            }

            // Dados dos veículos
            if (den.DenunciaVeiculo.Count > 0)
            {
                retorno.veiculos = new List<VeiculoModel>();
                den.DenunciaVeiculo.ToList().ForEach(dv =>
                    retorno.veiculos.Add(new VeiculoModel()
                    {
                        tipo_veiculo = dv.TipoVeiculo.DESCRICAO_TIPO_VEICULO,
                        placa = dv.NUMERO_PLACA,
                        marca = dv.MARCA_VEICULO,
                        cor_veiculo = dv.CorVeiculo.DESCRICAO_COR,
                        modelo = dv.MODELO_VEICULO,
                        observacoes = dv.OBSERVACOES,
                    })
                );
            }

            // Dados dos anexos
            if (den.DenunciaAnexo.Count > 0)
            {
                retorno.anexos = new List<AnexoModel>();
                den.DenunciaAnexo.ToList().ForEach(da =>
                    retorno.anexos.Add(new AnexoModel()
                    {
                        endereco = da.ENDERECO_ANEXO,
                        nome = da.ID_ANEXO.ToString(),
                        original = da.NOME_ORIGINAL,
                        extensao = da.MIME_TYPE,
                        observacoes = da.OBSERVACOES,
                        preview = (new List<string>() { ".bmp", ".gif", ".jpe", ".jpeg", ".jpg", ".png", ".tif" }.Any(t => t == da.MIME_TYPE)),
                    })
                );
            }

            // Dados das páginas de Internet
            if (den.DenunciaLink.Count > 0)
            {
                retorno.links = new List<LinkModel>();
                den.DenunciaLink.ToList().ForEach(dl =>
                    retorno.links.Add(new LinkModel()
                    {
                        endereco = dl.ENDERECO_LINK,
                        observacoes = dl.OBSERVACOES,
                    })
                );
            }

            return retorno;
        }

        #endregion

    }
}
