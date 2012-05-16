namespace EmailModule
{
    /// <summary>
    /// Entry point to the this email system
    /// </summary>
    public interface IEmailSystem
    {
        /// <summary>
        /// Send a mail message
        /// </summary>
        /// <param name="tempalteName">The template</param>
        /// <param name="model">The data</param>
        void SendMail(string tempalteName, object model);
    }
}