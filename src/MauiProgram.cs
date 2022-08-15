using PlexPlaylistExporter.ViewModels;
using PlexPlaylistExporter.Core.Services;
using PlexPlaylistExporter.Core.Contracts.Services;
using PlexPlaylistExporter.Services;
using PlexPlaylistExporter.Contracts;

namespace PlexPlaylistExporter;

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
			});

		// App Services
		builder.Services.AddSingleton<INavigationService, ShellNavigationService>();

        // Services
        builder.Services.AddTransient<IPlaylistService, OfflinePlaylistService>();

        // Pages
        builder.Services.AddTransient<AppShell>();

        builder.Services.AddTransient<MainPage>();
        
		builder.Services.AddTransient<PlaylistsPage>();
        builder.Services.AddTransient<PlaylistsViewModel>();

        builder.Services.AddTransient<PlaylistDetailPage>();
        builder.Services.AddTransient<PlaylistDetailViewModel>();

        return builder.Build();
	}
}
