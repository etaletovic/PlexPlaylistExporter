using PlexPlaylistExported.PlexClient.Contracts.Interfaces;
using PlexPlaylistExported.PlexClient.Contracts.Models;

namespace PlexPlaylistExported.PlexClient.Authentication
{
    public class AuthTokenAuthentication : IPlexAuthentication
    {
        private readonly string _token;

        public AuthTokenAuthentication(string token)
        {
            _token = token;
        }

        public async Task<IDictionary<string, string>> GetHeaders()
        {
            await Task.CompletedTask;
            return new Dictionary<string, string>
            {
                { PlexHeaders.XPlexToken, _token },
            };
        }
    }
}