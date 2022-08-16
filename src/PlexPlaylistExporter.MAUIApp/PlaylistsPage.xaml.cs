using PlexPlaylistExporter.MAUIApp.Contracts;
using PlexPlaylistExporter.MAUIApp.ViewModels;

namespace PlexPlaylistExporter.MAUIApp;

public partial class PlaylistsPage : ContentPage
{
	public PlaylistsPage(PlaylistsViewModel vm)
	{
		InitializeComponent();
		
		BindingContext = vm;
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		(BindingContext as INavigationAware)?.OnNavigatedTo();
	}

	protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
	{
		base.OnNavigatedFrom(args);

		(BindingContext as INavigationAware)?.OnNavigatedFrom();
    }
}