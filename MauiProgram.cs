using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace BudgetManager;

public static class MauiProgram
{
	//Expose the services to avoid constructor injection
	public static IServiceProvider Services { get; private set; } = null!;

	public static MauiApp CreateMauiApp()
	{
		//Gets the appsettings.json from the assembly
		Assembly assembly = Assembly.GetExecutingAssembly();
		using Stream? stream = assembly.GetManifestResourceStream("BudgetManager.appsettings.json");
		
		if (stream is null)
		{
			throw new InvalidOperationException("Embedded resource 'appsettings.json' not found.");
		}

		//Creates an Iconfiguration from the appsettiings.json to be injected
		IConfiguration config = new ConfigurationBuilder()
		.AddJsonStream(stream)
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
