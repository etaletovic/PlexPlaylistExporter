namespace PlexPlaylistExporter;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(Routes.PlaylistDetail, typeof(PlaylistDetailPage));

    }
}
