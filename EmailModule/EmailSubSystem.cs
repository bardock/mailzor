namespace EmailModule
{
    public class EmailSubsystem : IEmailSystem
    {
        public EmailSubsystem(string fromAddress, IEmailTemplateEngine templateEngine, IEmailSender sender)
        {
            Invariant.IsNotBlank(fromAddress, "fromAddress");
            Invariant.IsNotNull(templateEngine, "templateEngine");
            Invariant.IsNotNull(sender, "sender");

            FromAddress = fromAddress;
            TemplateEngine = templateEngine;
            Sender = sender;
        }

        protected IEmailTemplateEngine TemplateEngine { get; private set; }

        protected IEmailSender Sender { get; private set; }

        protected string FromAddress { get; private set; }

        /// <summary>
        /// Send a mail message
        /// </summary>
        /// <param name="tempalteName">The template</param>
        /// <param name="model">The data</param>
        public virtual void SendMail(string tempalteName, object model)
        {
            var mail = TemplateEngine.Execute(tempalteName, model);

            Sender.Send(mail);
        }
    }
}