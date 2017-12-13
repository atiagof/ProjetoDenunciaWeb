using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebDenunciaSSP.Atendimento.Controllers
{
    public class AtendimentoController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar,Atendimento Civil,Atendimento Militar,Policia Civil,Policia Militar")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar,Atendimento Civil,Atendimento Militar")]
        public ActionResult Analisar()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar,Atendimento Civil,Atendimento Militar")]
        public ActionResult AnaliseDenuncia()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar,Atendimento Civil,Atendimento Militar")]
        public ActionResult Validar()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar,Atendimento Civil,Atendimento Militar")]
        public ActionResult ValidaDenuncia()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar,Atendimento Civil,Atendimento Militar")]
        public ActionResult Descartar()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Administrador Civil,Administrador Militar,Atendimento Civil,Atendimento Militar")]
        public ActionResult DescarteDenuncia()
        {
            return View();
        }


    }
}