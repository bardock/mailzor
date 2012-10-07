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

    /*public interface IDoWhatRazorDoes
    {
        GeneratorResults GenerateCode(TextReader input);
        GeneratorResults GenerateCode(TextReader input, CancellationToken? cancelToken);
        GeneratorResults GenerateCode(TextReader input, string className, string rootNamespace, string sourceFileName);
    }*/

    public class RazorWrapper : IDoWhatRazorDoes
    {
        private static readonly RazorTemplateEngine RazorEngine = CreateRazorEngine();

        private static RazorTemplateEngine CreateRazorEngine()
        {
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultBaseClass = typeof(EmailTemplate).FullName,
                DefaultNamespace = "TempCompiledTemplates"
            };

            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Collections");
            host.NamespaceImports.Add("System.Collections.Generic");
            host.NamespaceImports.Add("System.Dynamic");
            host.NamespaceImports.Add("System.Linq");

            return new RazorTemplateEngine(host);
        }

        public SimplifiedParserResults GenerateCode(TextReader input)
        {
            return SimplifiedResults(RazorEngine.GenerateCode(input));
        }

        public SimplifiedParserResults GenerateCode(TextReader input, CancellationToken? cancelToken)
        {
            return SimplifiedResults(RazorEngine.GenerateCode(input, cancelToken));
        }

        public SimplifiedParserResults GenerateCode(TextReader input, string className, string rootNamespace, string sourceFileName)
        {
            return SimplifiedResults(RazorEngine.GenerateCode(input, className, rootNamespace, sourceFileName));
        }

        private SimplifiedParserResults SimplifiedResults(GeneratorResults generatorResults)
        {
            var simplifiedResults = new SimplifiedParserResults
            {
                ParserErrors = new List<AltRazorError>()
            };

            generatorResults.ParserErrors.ToList()
                .ForEach(e => simplifiedResults.ParserErrors.Add(new AltRazorError
                {
                    Location = e.Location.ToString(),
                    Message = e.Message
                }
                    ));

            return simplifiedResults;
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

            builder.RegisterType<RazorWrapper>().As<IDoWhatRazorDoes>();

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