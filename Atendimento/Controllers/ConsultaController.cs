using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebDenunciaSSP.Atendimento.Controllers
{
    public class ConsultaController : Controller
    {
        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar,Atendimento Civil,Atendimento Militar")]
        public ActionResult Consultar()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar,Atendimento Civil,Atendimento Militar")]
        public ActionResult ConsultarDenuncia()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar")]
        public ActionResult DenunciaDashboard()
        {
            return View();
        }
    }
}