using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Extensions;
using WebDenunciaSSP.CommonLibs;

namespace WebDenunciaSSP.Atendimento
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.MapHttpAttributeRoutes();

            var odataBuilder = OdataBuilder.registroOData();

            var model = odataBuilder.GetEdmModel();

            config.MapODataServiceRoute(
                 routeName: "ODataRoute",
                 routePrefix: "odata",
                 model: model);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
