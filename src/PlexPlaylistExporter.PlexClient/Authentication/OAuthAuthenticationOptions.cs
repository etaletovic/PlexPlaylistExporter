namespace PlexPlaylistExporter.PlexClient.Authentication
{
    public class OAuthAuthenticationOptions
    {
        public Action<string> SignInUrlCallback { get; set; }
        public string ClientId { get; set; }
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
