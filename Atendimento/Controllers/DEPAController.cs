using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDenunciaSSP.Entidades.Context;

namespace WebDenunciaSSP.Atendimento.Controllers
{
    public class DEPAController : Controller
    {
        private readonly WDContext _dbContext;

        public DEPAController(WDContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: DEPA
        public ActionResult Denuncia()
        {            
            return View();
        }
    }
}