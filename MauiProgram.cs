using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BudgetManager;

public static class MauiProgram
{
	//Expose the services to avoid constructor injection
	public static IServiceProvider Services { get; private set; }
	public static MauiApp CreateMauiApp()
	{
		//Creates an Iconfiguration from the appsettiings.json to be injected
		IConfiguration config = new ConfigurationManager()
		.SetBasePath(Environment.CurrentDirectory)
		.AddJsonFile("appsettings.json")
		.Build();

		MauiAppBuilder builder = MauiApp.CreateBuilder();

		//Adds the Iconfiguration to the app
		builder.Configuration.AddConfiguration(config);
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
		Services = builder.Services.BuildServiceProvider();
		return builder.Build();
	}
}
