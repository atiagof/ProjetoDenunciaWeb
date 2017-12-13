using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebDenunciaSSP.Entidades.Context;

namespace WebDenunciaSSP.Atendimento.App_Start
{
    public class StartupOWIN
    {
        public static void Configure(IAppBuilder app, HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            // Listar os assemblies
            List<Assembly> assemblies = new List<Assembly>();
            assemblies.Add(typeof(WebDenunciaSSP.Atendimento.Global).Assembly);
            assemblies.Add(typeof(WDContext).Assembly);
            assemblies.Add(typeof(WebDenunciaSSP.CommonLibs.OData.CicatrizController).Assembly);

            // Registrar os assemblies
            foreach (Assembly assembly in assemblies)
            {
                builder.RegisterControllers(assembly);
                builder.RegisterApiControllers(assembly);
                builder.RegisterModelBinders(assembly);
            }

            // Carregar serviços

            // Autenticação OWIN
            builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();
            builder.Register<IOwinContext>(c => HttpContext.Current.GetOwinContext()).InstancePerRequest();

            // Registrar contexto do banco
            builder.Register<WDContext>(c => new WDContext()).InstancePerLifetimeScope();

            builder.RegisterModelBinderProvider();
            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());
            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);


            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                CookieName = CookieAuthenticationDefaults.AuthenticationType,
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,//"ApplicationCookie",//DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Atendimento/Login"),
                ExpireTimeSpan = TimeSpan.FromDays(7),
                SlidingExpiration = true,
                Provider = new CookieAuthenticationProvider
                {
                    OnResponseSignIn = ctx => {
                        var ticks = ctx.Options.SystemClock.UtcNow.AddHours(10).UtcTicks;
                        ctx.Properties.Dictionary.Add("absolute", ticks.ToString());
                    },
                    OnValidateIdentity = ctx =>
                    {
                        bool reject = true;
                        string value;
                        if (ctx.Properties.Dictionary.TryGetValue("absolute", out value))
                        {
                            long ticks;
                            if (Int64.TryParse(value, out ticks))
                            {
                                reject = ctx.Options.SystemClock.UtcNow.UtcTicks > ticks;
                            }
                        }

                        if (reject)
                        {
                            ctx.RejectIdentity();
                            // optionally clear cookie
                            ctx.OwinContext.Authentication.SignOut(ctx.Options.AuthenticationType);
                        }

                        return Task.FromResult(0);
                    }
                }
            });


            // OWIN MVC SETUP:
            // Register the Autofac middleware FIRST, then the Autofac MVC middleware.
            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();
            app.UseAutofacWebApi(config);
        }
    }
}