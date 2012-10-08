using System;
using System.IO;
using System.Net.Mail;

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
            return new SmtpClient("127.0.0.1", 25);
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
}
