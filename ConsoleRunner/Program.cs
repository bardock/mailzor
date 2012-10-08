using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using System.Web.Razor;

using EmailModule;

namespace ConsoleRunner
{

    public class Program
    {
        public static void Main()
        {
            var loc = @"..\..\..\Templates";

            Execute(loc);

            Console.WriteLine("Mail delivered, check the outbox folder (if using the local file drop), or check recipient email (if using real smtp).");
            Console.Read();
        }

        public static void Execute(String templateLocation)
        {
            IEmailTemplateContentReader templateReader = new FileSystemEmailTemplateContentReader(templateLocation);
            IEmailTemplateEngine templateEngine = new EmailTemplateEngine(templateReader, new RazorWrapper(), RazorWrapper.BuildReferenceList());

            IEmailSender sender = new EmailSender
            {
                // replace with RealSmtpClient() to test with a real mail server
                CreateClientFactory = () => new SmtpClientWrapper(RealSmtpClient())
            };

            var subsystem = new EmailSubsystem("daedalus@adcastgroup.com", templateEngine, sender);

            subsystem.SendMail(
                "NewTaskEmailCommand",
                new NewTaskEmailCommand
                {
                    To = "nick@adcastgroup.com",
                    From = "daedalus@adcastgroup.com"
                });
        }

        private static SmtpClient RealSmtpClient()
        {
            // replace this with your own mail server
            return new SmtpClient("10.1.0.82", 25);
        }

        private static SmtpClient CreateSmtpClientWhichDropsInLocalFileSystem()
        {
            var outbox = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "outbox");

            if (!Directory.Exists(outbox))
            {
                Directory.CreateDirectory(outbox);
            }

            return new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = outbox,
                Host = "localhost",
                UseDefaultCredentials = true
            };
        }
    }

    public class NewTaskEmailCommand
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Name { get; set; }

        public string VerificationUri { get; set; }
    }

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

        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static IEnumerable<string> BuildReferenceList()
        {
            //var currentAssemblyLocation = typeof(EmailTemplateEngine).Assembly.CodeBase.Replace("file:///", string.Empty).Replace("/", "\\");

            return new List<string>
                       {
                           "mscorlib.dll",
                           "system.dll",
                           "system.core.dll",
                           "microsoft.csharp.dll",
                           Assembly.GetExecutingAssembly().Location
                       };
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
}
