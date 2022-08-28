using Foundation;
using PlexPlaylistExporter.MAUIApp;

namespace PlexPlaylistExporter.MAUIApp.Platforms.iOS;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
