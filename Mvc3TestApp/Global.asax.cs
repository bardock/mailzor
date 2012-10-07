using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Autofac;
using Autofac.Integration.Mvc;

using EmailModule;

namespace Mvc3TestApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Use LocalDB for Entity Framework by default
            Database.DefaultConnectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");
            
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);


            var container = BuildContainer();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        protected static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterModule(new MailzorModule
            {
                TemplatesDirectory = @"..\Templates",
                SmtpServerIp = "127.0.0.1", // your smtp server
                SmtpServerPort = 25
            });

            return builder.Build();
        }
    }


    public class MailzorModule : Autofac.Module
    {
        public string TemplatesDirectory { get; set; }
        public string SmtpServerIp { get; set; }
        public int SmtpServerPort { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(
                    c => new FileSystemEmailTemplateContentReader(TemplatesDirectory))
                .As<IEmailTemplateContentReader>();

            builder
                .RegisterType<EmailTemplateEngine>()
                .As<IEmailTemplateEngine>();

            builder
                .Register(
                    c => new EmailSender
                    {
                        CreateClientFactory = ()
                            => new SmtpClientWrapper(new SmtpClient(SmtpServerIp, SmtpServerPort))
                    })
                .As<IEmailSender>();

            builder
                .Register(
                    c => new EmailSubsystem(
                        "sending@from-site.com",
                        c.Resolve<IEmailTemplateEngine>(),
                        c.Resolve<IEmailSender>()))
                .As<IEmailSystem>();
        }
    }
}