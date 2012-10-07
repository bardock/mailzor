using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EmailModule;

namespace Mvc3TestApp.Controllers
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
            ViewBag.Message = "Click on the about page to send an email";

            return View();
        }

        public ActionResult About()
        {
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
