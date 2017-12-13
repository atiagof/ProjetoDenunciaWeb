using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDenunciaSSP.CommonLibs.Models;

namespace WebDenunciaSSP.Atendimento.Controllers
{
    public class DirectiveController : Controller
    {
        // GET: Directive
        public ActionResult SelectOneFromEntity(string entityName, string textField, string valueField)
        {
            SelectOneFromEntityModel model = new SelectOneFromEntityModel();

            model.EntityName = entityName;
            model.TextField = textField;
            model.ValueField = valueField;

            return PartialView(model);
        }
    }
}