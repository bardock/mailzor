using System;
using System.IO;
using System.Net.Mail;

using EmailModule;

namespace ConsoleRunner
{
    internal class Program
    {
        public static void Main()
        {
            IEmailTemplateContentReader templateReader = new FileSystemEmailTemplateContentReader(@"..\..\Templates");
            IEmailTemplateEngine templateEngine = new EmailTemplateEngine(templateReader);

            IEmailSender sender = new EmailSender
            {
                // replace with RealSmtpClient() to test with a real mail server
                CreateClientFactory = () => new SmtpClientWrapper(CreateSmtpClientWhichDropsInLocalFileSystem())
            };

            var subsystem = new EmailSubsystem("someone@somewhere.com", templateEngine, sender);

            subsystem.SendMail(
                "TaskNotificationMessage",
                new TaskNotificationMessage
                    {
                        To = "someone@somewhere.com", From = "source@somewhere.com"
                    });

            Console.WriteLine("Mail delivered, check the outbox folder.");
            Console.Read();
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
}
