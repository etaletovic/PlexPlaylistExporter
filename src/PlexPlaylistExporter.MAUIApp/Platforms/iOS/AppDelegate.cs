using Foundation;
using PlexPlaylistExporter.MAUIApp;

namespace PlexPlaylistExporter.App.Platforms.iOS;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
