using PlexPlaylistExporter.Core.Contracts.Models;

namespace PlexPlaylistExporter.Core.Contracts.Services
{
    public interface IPlaylistService
    {
        Task<IEnumerable<Playlist>> GetPlaylists();
    }
}
