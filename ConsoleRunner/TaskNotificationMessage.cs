using System;

namespace ConsoleRunner
{
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

    /*public class NewTaskEmailCommand
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Name { get; set; }

        public string VerificationUri { get; set; }
    }*/
}