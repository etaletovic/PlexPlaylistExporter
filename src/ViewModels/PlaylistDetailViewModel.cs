using CommunityToolkit.Mvvm.ComponentModel;
using PlexPlaylistExporter.Core.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexPlaylistExporter.ViewModels
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
