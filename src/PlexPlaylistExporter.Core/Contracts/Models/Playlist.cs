namespace PlexPlaylistExporter.Core.Contracts.Models
{
    public class Playlist
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ThumbUrl { get; set; }

        public List<Track> Tracks { get; set; }
    }
}
