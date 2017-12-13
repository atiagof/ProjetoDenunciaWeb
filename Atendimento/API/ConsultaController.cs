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
    public class ConsultaController : ApiController
    {
        private readonly WDContext _dbContext;

        public ConsultaController(WDContext context)
        {
            _dbContext = context;
        }

        #region GET

        [HttpGet]
        // Abre as informações da denúncia de acordo com o perfil do usuário
        public IHttpActionResult abrirDenunciaMacro()
        {
            DenunciaDashboardViewModel macro = new DenunciaDashboardViewModel();

            try
            {
                macro.denunciasTotal = _dbContext.Denuncia.Count(c => c.NUM_DENUNCIA > 0);

                macro.denunciasNaoAtendidasTotal = _dbContext.Denuncia.Count(d => d.ID_SITUACAO_PM == 1 && d.ID_SITUACAO_PC == 1);

                macro.denunciasPM = _dbContext.Denuncia.Count(d => d.ID_SITUACAO_PM != 1);
                macro.denunciasPMAguardando = _dbContext.Denuncia.Count(d => d.ID_SITUACAO_PM == 1);

                macro.denunciasPC = _dbContext.Denuncia.Count(d => d.ID_SITUACAO_PC != 1);
                macro.denunciasPCAguardando = _dbContext.Denuncia.Count(d => d.ID_SITUACAO_PC == 1);

                macro.denunciasEmAnalisePM = _dbContext.Denuncia.Count(x => (new List<int>() { 2, 3, 5, 6, 7, 9 }).Any(s => x.ID_SITUACAO_PM == s));

                macro.denunciasEmAnalisePC = _dbContext.Denuncia.Count(x => (new List<int>() { 2, 3, 5, 6, 7, 9 }).Any(s => x.ID_SITUACAO_PC == s));

                macro.denunciasDescatardasPM = _dbContext.Denuncia.Count(d => d.ID_SITUACAO_PM == 4);

                macro.denunciasDescatardasPC = _dbContext.Denuncia.Count(d => d.ID_SITUACAO_PC == 4);

                macro.denunciasFinalizadasPositivaPM = _dbContext.Denuncia.Count(d => d.ID_SITUACAO_PM == 8 && d.FLAG_RESPOSTA_POSITIVA_PM == true);
                macro.denunciasFinalizadasNegativaPM = _dbContext.Denuncia.Count(d => d.ID_SITUACAO_PM == 8 && d.FLAG_RESPOSTA_POSITIVA_PM == false);

                macro.denunciasFinalizadasPositivaPC = _dbContext.Denuncia.Count(d => d.ID_SITUACAO_PC == 8 && d.FLAG_RESPOSTA_POSITIVA_PC == true);
                macro.denunciasFinalizadasNegativaPC = _dbContext.Denuncia.Count(d => d.ID_SITUACAO_PC == 8 && d.FLAG_RESPOSTA_POSITIVA_PC == false);

                return Ok(macro);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        // 
        public IHttpActionResult pesquisaAvancada()
        {
            try
            {

                return Ok();
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("Um erro ocorreu ao executar a pesquisa:")
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

    }
}
