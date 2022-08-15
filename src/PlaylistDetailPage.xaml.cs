using PlexPlaylistExporter.ViewModels;

namespace PlexPlaylistExporter;

public partial class PlaylistDetailPage : ContentPage
{
	public PlaylistDetailPage(PlaylistDetailViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}