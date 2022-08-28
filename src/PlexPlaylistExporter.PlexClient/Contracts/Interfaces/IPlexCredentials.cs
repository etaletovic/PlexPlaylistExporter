namespace PlexPlaylistExporter.PlexClient.Contracts.Interfaces
{
    public interface IPlexCredentials
    {
        string GetToken();
        void SetToken(string token);
    }
}