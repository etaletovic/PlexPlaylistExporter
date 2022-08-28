using Android.App;
using Android.Runtime;
using PlexPlaylistExporter.MAUIApp;

namespace PlexPlaylistExporter.MAUIApp.Platforms.Android;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
