namespace PlexPlaylistExporter.Core.Contracts.Models
{
    public class Track
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public TimeSpan Duration { get; set; }

        public Album Album { get; set; }
    }
}
