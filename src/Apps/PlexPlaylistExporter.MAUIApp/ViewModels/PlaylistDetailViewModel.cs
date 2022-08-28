using CommunityToolkit.Mvvm.ComponentModel;
using PlexPlaylistExporter.Core.Contracts.Models;

namespace PlexPlaylistExporter.MAUIApp.ViewModels
{
    [ObservableObject]
    public partial class PlaylistDetailViewModel : IQueryAttributable
    {
        [ObservableProperty]
        private Playlist _playlist;

        [ObservableProperty]
        private Track _selectedTrack;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Playlist = query[nameof(Playlist)] as Playlist;
        }
    }
}
