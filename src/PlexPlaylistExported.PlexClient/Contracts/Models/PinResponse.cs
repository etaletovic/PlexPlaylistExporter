namespace PlexPlaylistExporter.PlexClient.Contracts.Models
{
    public class PinResponse
    {
        public int id { get; set; }
        public string code { get; set; }
        public string product { get; set; }
        public bool trusted { get; set; }
        public string clientIdentifier { get; set; }
        public Location location { get; set; }
        public int expiresIn { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime expiresAt { get; set; }
        public string authToken { get; set; }
        public bool? newRegistration { get; set; }
    }
}
