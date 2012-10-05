# mailzor

Is a basic utility class to help generate and send emails using the Razor view engine to create email templates, quickly pluggable into your .NET app.

The original idea for this is from , with the 

 - Original blog post covering it here:
  - [kazimanzurrashid.com/use-razor-for-email-template-outside-asp-dot-net-mvc]( http://kazimanzurrashid.com/posts/use-razor-for-email-template-outside-asp-dot-net-mvc )
 - Original code
   - [EmailTemplate.zip]( http://media.kazimanzurrashid.s3.amazonaws.com/EmailTemplate.zip )

### Limitations
  One right now is compiled dependency on System.Web.Razor 2.0.20126.16343 which is bundled with the package, see troubleshooting section at the bottom.

## NuGet

 Get it from [nuget.org/packages/mailzor](https://nuget.org/packages/mailzor) or via Package Manager Console
 
  > *PM> Install-Package mailzor*

# Usage

	IEmailSystem mailzor;
	
	mailzor.SendMail(
		new TaskNotificationMessage
                    {
                        To = "email@domain.com",
                        From = "source@domain.com"
                    });


## IoC Wireup

Using an Autofac module (or just using this registration code in your composition root) to wire up the dependencies for the `mailzor` utility.
	
	builder.RegisterModule(new MailzorModule 
		{ 
			TemplatesDirectory = @"..\Templates",
			SmtpServerIp = "10.0.0.99", // your smtp server
			SmtpServerPort = 25
		});

### Autofac

	public class MailzorModule : Autofac.Module
	{
		public string TemplatesDirectory { get; set; }
		public string SmtpServerIp { get; set; }
		public int SmtpServerPort { get; set; }
	
		protected override void Load(ContainerBuilder builder)
		{
			builder
				.Register(
					c => new FileSystemEmailTemplateContentReader(TemplatesDirectory))
				.As<IEmailTemplateContentReader>();
	
			builder
				.RegisterType<EmailTemplateEngine>()
				.As<IEmailTemplateEngine>();
	
			builder
				.Register(
					c => new EmailSender
						{
							CreateClientFactory = () 
								=> new SmtpClientWrapper(new SmtpClient(SmtpServerIp, SmtpServerPort))
						})
				.As<IEmailSender>();
	
			builder
				.Register(
					c => new EmailSubsystem(
						"sending@from-site.com", 
						c.Resolve<IEmailTemplateEngine>(), 
						c.Resolve<IEmailSender>()))
				.As<IEmailSystem>();
		}
	}

## Changes in this fork

 - A single entry point via `IEmailSystem`
 - Send message via `SendMail` on `IEmailSystem`
 - Some additional template loading checking, to ensure they're available and that it reports when it can't find them (in particular which template it couldn't find).
 
## Troubleshooting
 - Because this library takes a fixed dependency on a version of 'System.Web.Razor' version: 2.0.20126.16343 so if you see an exception that's references a 'WriteAttribute' check for [exception image](http://yfrog.com/kksyjsp)
   - *.config binding redirects for System.Web.Razor
   - Any conflicting versions of System.Web.Razor
 
	> 	The name 'WriteAttribute' does not exist in the current context
	
## License
Licensed under the MIT license.