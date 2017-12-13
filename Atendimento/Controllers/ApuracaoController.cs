using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebDenunciaSSP.Atendimento.Controllers
{
    public class ApuracaoController : Controller
    {
        // GET: Apuracao
        [Authorize(Roles = "Administrador,Policia Civil,Policia Militar")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Policia Civil,Policia Militar")]
        public ActionResult Apurar()
        {
            return View();
        }

        //ToDo: Em teste
        //[Authorize(Roles = "Administrador,Policia Civil,Policia Militar")]
        public ActionResult ApurarNovo()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Policia Civil,Policia Militar")]
        public ActionResult ApuraDenuncia()
        {
            return View();
        }
    }
}