namespace PlexPlaylistExporter.Core.Contracts.Models
{
    public class Album
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public List<Track> Tracks { get; set; }
    }
}
