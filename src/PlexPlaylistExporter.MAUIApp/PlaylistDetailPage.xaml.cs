using PlexPlaylistExporter.MAUIApp.ViewModels;

namespace PlexPlaylistExporter.MAUIApp;

public partial class PlaylistDetailPage : ContentPage
{
	public PlaylistDetailPage(PlaylistDetailViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}