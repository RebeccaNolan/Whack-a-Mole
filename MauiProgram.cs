using Plugin.Maui.Audio;

namespace Nolan_Rebecca_VS22;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Debrosee-ALPnL.ttf", "DebroseeALPnL");
            });

        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
	}
}
