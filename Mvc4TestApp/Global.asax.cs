using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Razor;
using System.Web.Routing;

using Autofac;
using Autofac.Integration.Mvc;

using EmailModule;

namespace Mvc4TestApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

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