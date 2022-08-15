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
        public async Task<IEnumerable<Playlist>> GetPlaylists()
        {
            await Task.CompletedTask;

            return Enumerable.Range(1, 10).Select(x => new Playlist { Id = x.ToString(), Name = $"Playlist {x}" });
        }
    }
}
