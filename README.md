# mailzor

Is a basic utility class to help generate and send emails using the Razor view engine to create email templates, quickly pluggable into your .NET app.

The original idea for this is from , with the 

 - Original blog post covering it here:
  - [kazimanzurrashid.com/use-razor-for-email-template-outside-asp-dot-net-mvc]( http://kazimanzurrashid.com/posts/use-razor-for-email-template-outside-asp-dot-net-mvc )
 - Original code
   - [EmailTemplate.zip]( http://media.kazimanzurrashid.s3.amazonaws.com/EmailTemplate.zip )

## NuGet

 **Version 1.0.0.10**
 
 Get it from [nuget.org/packages/mailzor](https://nuget.org/packages/mailzor) or via Package Manager Console
 
  > *PM> Install-Package mailzor*

## Testing

In the repository open \ExperienceTesting\Mvc4TestApp\Mvc4TestApp.sln - it makes use of the nuget package.

Have [smpt4dev](http://smtp4dev.codeplex.com/) running or configure it to a real mail server and see it deliver a test message.

 - Functions with:
   - Razor 1.0 in ASP.NET MVC 3
   - Razor 2.0 in ASP.NET MVC 4

## Building from source

Run build.bat which calls out to a [psake](http://en.wikipedia.org/wiki/Psake) script `mailzor-build.ps1`. The dependant Razor assembly will be ilmerged as part of the build.


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
			SmtpServerIp = "127.0.0.1", // your smtp server
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


## Older version
 
If you tried this prior to 1.0.0.10 there was an issue with incompatible razor versions, more info [here](https://github.com/NickJosevski/mailzor/blob/master/PriorBugs.md).

## License
Licensed under the MIT license.




