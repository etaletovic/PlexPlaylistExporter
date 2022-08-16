using CommunityToolkit.Mvvm.ComponentModel;
using PlexPlaylistExporter.Core.Contracts.Models;
using PlexPlaylistExporter.Core.Contracts.Services;
using PlexPlaylistExporter.MAUIApp.Contracts;
using System.Collections.ObjectModel;

namespace PlexPlaylistExporter.MAUIApp.ViewModels
{
    [ObservableObject]
    public partial class PlaylistsViewModel : INavigationAware
    {
        private readonly IPlaylistService _playlistService;
        private readonly INavigationService _navigationService;

        public PlaylistsViewModel(IPlaylistService playlistService, INavigationService navigationService)
        {
            _playlistService = playlistService ?? throw new ArgumentNullException(nameof(playlistService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            Playlists = new();
        }

        public ObservableCollection<Playlist> Playlists { get; set; }

        [ObservableProperty]
        private Playlist selected;

        partial void OnSelectedChanged(Playlist value)
        {
            if (value is null) return;

            _navigationService.GoTo(Routes.PlaylistDetail, new Dictionary<string, object> {
                { nameof(Playlist), Selected }
            });
        }

        public async Task OnNavigatedFrom()
        {
            await Task.CompletedTask;

            Playlists.Clear();
        }

        public async Task OnNavigatedTo()
        {
            await Task.CompletedTask;

            var playlists = await _playlistService.GetPlaylists().ConfigureAwait(false);
            foreach (var playlist in playlists)
                Playlists.Add(playlist);
        }
    }
}
