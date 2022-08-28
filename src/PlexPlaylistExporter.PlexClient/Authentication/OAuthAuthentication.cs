using System.Text.Json;
using System.Web;
using PlexPlaylistExporter.PlexClient.Contracts.Interfaces;
using PlexPlaylistExporter.PlexClient.Contracts.Models;

namespace PlexPlaylistExporter.PlexClient.Authentication
{
    public class OAuthAuthentication : IPlexAuthentication, IDisposable
    {
        const string NewPinURL = "https://plex.tv/api/v2/pins";
        const string PinStatusURL = "https://plex.tv/api/v2/pins/{0}";
        private readonly string _clientId;
        private readonly Action<string> _promptUrlSignInCallback;
        private readonly TimeSpan _timeout;
        private readonly HttpClient _client;
        private bool disposedValue;

        private PinResponse? _authToken;
        public OAuthAuthentication(string clientId, Action<string> promptUrlSignInCallback, TimeSpan timeout = default)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentException($"'{nameof(clientId)}' cannot be null or whitespace.", nameof(clientId));
            }

            _timeout = timeout;
            if (_timeout == default)
                _timeout = TimeSpan.FromSeconds(30);

            _client = new HttpClient();
            _clientId = clientId;
            _promptUrlSignInCallback = promptUrlSignInCallback;
        }

        public async Task<IDictionary<string, string>> GetHeaders()
        {
            await RefreshTokenIfNecessary();

            var token = _authToken?.authToken ?? throw new InvalidOperationException("Token is null");

            return new Dictionary<string, string>
            {
                { PlexHeaders.XPlexToken, token }
            };
        }

        private bool IsTokenExpired(PinResponse? token)
        {
            if (token is null) return true;
            if (token.trusted == false) return true;
            if (token.expiresAt > DateTime.Now.AddMinutes(-5)) return true;

            return false;
        }

        private async Task RefreshTokenIfNecessary()
        {
            await Task.CompletedTask;

            if (IsTokenExpired(_authToken) == false) return;

            await RefreshToken();
        }

        private async Task RefreshToken()
        {
            await Task.CompletedTask;

            _authToken = null;
            // Create new PIN
            using var newPinMessage = new HttpRequestMessage(HttpMethod.Post, NewPinURL);
            newPinMessage.Headers.Add(PlexHeaders.XPlexProduct, "Plex Playlist Exporter");
            newPinMessage.Headers.Add(PlexHeaders.XPlexClientIdentifier, _clientId);
            newPinMessage.Headers.Add("Accept", "application/json");
            newPinMessage.Headers.Add(PlexHeaders.Strong, "true");

            using var newPinResponse = await _client.SendAsync(newPinMessage);
            var responseString = await newPinResponse.Content.ReadAsStringAsync();
            var pin = JsonSerializer.Deserialize<PinResponse>(responseString);

            if (pin is null)
                throw new InvalidOperationException("Could not generate PIN to claim");

            // Build login URL for the browser
            var loginBaseString = "https://app.plex.tv/auth#";
            var clientID = $"clientID={_clientId}";
            var code = $"code={pin.code}";
            var appName = $"{HttpUtility.UrlEncode("context[device][product]")}={HttpUtility.UrlEncode("Plex Web")}";
            var appVersion = $"{HttpUtility.UrlEncode("context[device][version]")}=v1";
            var device = $"{HttpUtility.UrlEncode("context[device][device]")}=Plex";
            var forwardUrl = $"forwardUrl={HttpUtility.UrlEncode("https://app.plex.tv")}";
            var loginURL = $"{loginBaseString}?{clientID}&{appName}&{appVersion}&{device}&{forwardUrl}&{code}";

            _promptUrlSignInCallback.Invoke(loginURL);

            var timeoutAt = DateTime.Now.Add(_timeout);
            var formData = new Dictionary<string, string>
            {
                { "code", pin.code },
                { PlexHeaders.XPlexClientIdentifier, _clientId },
            };

            while (DateTime.Now < timeoutAt)
            {
                using var pinStatusMessage = new HttpRequestMessage(HttpMethod.Get, string.Format(PinStatusURL, pin.id));
                using var formContent = new FormUrlEncodedContent(formData);
                pinStatusMessage.Content = formContent;

                pinStatusMessage.Headers.Add("Accept", "application/json");

                using var pinStatusResponse = await _client.SendAsync(pinStatusMessage);
                var tokenResponseString = await pinStatusResponse.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<PinResponse>(tokenResponseString);

                var gotToken = string.IsNullOrWhiteSpace(token?.authToken) == false;

                if (!gotToken)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    continue;
                }

                _authToken = token;
                break;
            }

            throw new InvalidOperationException("Failed to obtain auth token");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (disposing)
            {
                // Dispose managed state (managed objects)
                _client?.Dispose();
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer
            // Set large fields to null
            disposedValue = true;
        }

        // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~OAuthAuthentication()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}