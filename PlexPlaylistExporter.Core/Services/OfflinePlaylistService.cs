using PlexPlaylistExporter.Core.Contracts.Models;
using PlexPlaylistExporter.Core.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexPlaylistExporter.Core.Services
{
    public class OfflinePlaylistService : IPlaylistService
    {
        private static readonly string OfflinePlaylistsLocation = @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Plexamp\Plexamp\Offline\";

        public async Task<IEnumerable<Playlist>> GetPlaylists()
        {
            await Task.CompletedTask;

            if (!Directory.Exists(OfflinePlaylistsLocation))
                return Enumerable.Empty<Playlist>();

            return Directory.GetDirectories(OfflinePlaylistsLocation)
                            .Select(x => new DirectoryInfo(x))
                            .Select( x =>
                            {
                                var thumbnail = x.GetFiles().FirstOrDefault(f => f.Name.Equals("thumb.jpg", StringComparison.OrdinalIgnoreCase));
                                return new Playlist
                                {
                                    Id = x.Name,
                                    Name = $"Unknown Playlist",
                                    ThumbUrl = thumbnail is null ? string.Empty : thumbnail.FullName,
                                    Tracks = GetTracks(x.Name)
                                };
                            });
        }

        private List<Track> GetTracks(string playlistId)
        {

            var playlist = Path.Combine(OfflinePlaylistsLocation, playlistId);
            var index = Path.Combine(playlist, "index.json");

            if (!Directory.Exists(playlist) || !File.Exists(index))
                return Enumerable.Empty<Track>().ToList();

            return Enumerable.Empty<Track>().ToList();

            var files = Directory.GetFiles(playlist);
        }
    }
}
