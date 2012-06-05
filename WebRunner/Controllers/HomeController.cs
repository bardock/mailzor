using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

using ConsoleRunner;

using EmailModule;

namespace WebRunner.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to kick-start your ASP.NET MVC application.";

            // Just kick off the console app logic
            // Program.Execute(@"..\Templates");

            // host the logic here
            Execute(@"..\Templates");

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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

            var subsystem = new EmailSubsystem("src@domain.com", templateEngine, sender);

            subsystem.SendMail(
                "NewTaskEmailCommand",
                new NewTaskEmailCommand
                {
                    To = "user@domain.com",
                    From = "src@domain.com"
                });
        }

        public class NewTaskEmailCommand
        {
            public string From { get; set; }

            public string To { get; set; }

            public string Name { get; set; }

            public string VerificationUri { get; set; }
        }

        private static SmtpClient RealSmtpClient()
        {
            // replace this with your own mail server
            return new SmtpClient("10.1.0.82", 25);
        }
    }
}
