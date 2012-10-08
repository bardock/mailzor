using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EmailModule;

namespace Mvc4TestApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailSystem _mailzor;

        public HomeController(IEmailSystem mailzor)
        {
            _mailzor = mailzor;
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Mailzor mvc4 test";

            return View();
        }

        public ActionResult SendEmail()
        {
            ViewBag.Message = "Check the smtp4dev window.";

            _mailzor.SendMail("TaskNotificationMessage",
                new TaskNotificationMessage
                {
                    To = "email@domain.com",
                    From = "source@domain.com"
                });

            return View();
        }
    }
    public class TaskNotificationMessage
    {
        public string EmailAddress { get; set; }

        public DateTime Deadline { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Name { get; set; }

        public string VerificationUri { get; set; }

        public string LogOnUrl { get; set; }
    }
}
