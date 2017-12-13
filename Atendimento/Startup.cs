using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using WebDenunciaSSP.Atendimento.App_Start;
using System.Web.Http;

[assembly: OwinStartup(typeof(WebDenunciaSSP.Atendimento.Startup))]

namespace WebDenunciaSSP.Atendimento
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            StartupOWIN.Configure(app, GlobalConfiguration.Configuration);
        }
    }
}