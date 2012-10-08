using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using EmailModule;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            Execute(@"..\..\..\..\..\Templates");
        }

        public static void Execute(String templateLocation)
        {
            IEmailTemplateContentReader templateReader = new FileSystemEmailTemplateContentReader(templateLocation);
            IEmailTemplateEngine templateEngine = new EmailTemplateEngine(templateReader);

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
    }

    public class NewTaskEmailCommand
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Name { get; set; }

        public string VerificationUri { get; set; }
    }
}
