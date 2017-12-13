using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using WebDenunciaSSP.Atendimento.Models;
using WebDenunciaSSP.CommonLibs.Models;
using WebDenunciaSSP.Entidades;
using WebDenunciaSSP.Entidades.Context;

namespace WebDenunciaSSP.Atendimento.API
{
    public class LogController : ApiController
    {

        private readonly WDContext _dbContext;

        public LogController(WDContext context)
        {
            _dbContext = context;
        }

        #region GET

        #endregion

        #region POST

        [HttpPost]
        // Salva logs relacionados ao usuário do atendimento
        public IHttpActionResult salvarLogUsuario(int idUsuario, int idUsuarioAlterado, string descricao)
        {
            try
            {
                UsuarioLog log = new UsuarioLog();

                log.ID_USUARIO = idUsuario;
                log.ID_USUARIO_ALTERADO = idUsuarioAlterado > 0 ? idUsuarioAlterado : (int?)null;
                log.DATA_LOG = DateTime.Now;
                log.DESCRICAO_LOG = descricao;

                _dbContext.UsuarioLog.Add(log);
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("Um erro ocorreu ao salvar o log de usuário:")
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

        [HttpPost]
        // Salva logs relacionados ao trabalho da denúncia no atendimento
        public IHttpActionResult salvarLogDenuncia(int ano, int numero, int idUsuario, string descricao)
        {
            try
            {
                DenunciaLog log = new DenunciaLog();

                log.ANO_DENUNCIA = ano;
                log.NUM_DENUNCIA = numero;
                log.ID_USUARIO = idUsuario;
                log.DATA_LOG = DateTime.Now;
                log.DESCRICAO_LOG = descricao;

                _dbContext.DenunciaLog.Add(log);
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("Um erro ocorreu ao salvar o log da denúncia:")
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

    }
}
