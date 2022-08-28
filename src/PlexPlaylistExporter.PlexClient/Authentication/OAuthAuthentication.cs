using System.Net.NetworkInformation;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Options;
using PlexPlaylistExporter.PlexClient.Contracts.Interfaces;
using PlexPlaylistExporter.PlexClient.Contracts.Models;
using PlexPlaylistExporter.SecretsManager.Abstractions.Contracts;

namespace PlexPlaylistExporter.PlexClient.Authentication
{
    public class OAuthAuthentication : IPlexAuthentication, IDisposable
    {
        private const string NewPinURL = "https://plex.tv/api/v2/pins";
        private const string PinStatusURL = "https://plex.tv/api/v2/pins/{0}";
        private const string SecretTokenId = "plex-exporter-token-id";
        private const string SecretTokenCode = "plex-exporter-code";

        private readonly HttpClient _client;
        private readonly ISecretsManager _secretsManager;
        private readonly OAuthAuthenticationOptions _options;

        private bool _disposedValue;
        private bool _disposeClient;

        private PinResponse? _authToken;

        public OAuthAuthentication(IOptions<OAuthAuthenticationOptions> options)
            : this(options, null) { }

        public OAuthAuthentication(IOptions<OAuthAuthenticationOptions> options,
                                   ISecretsManager secretsManager)
            : this(options, new HttpClient(), secretsManager)
        {
            _disposeClient = true;
        }

        public OAuthAuthentication(IOptions<OAuthAuthenticationOptions> options,
                                   HttpClient client,
                                   ISecretsManager secretsManager)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            _client = client ?? throw new ArgumentNullException(nameof(client));
            _secretsManager = secretsManager;
            _options = options.Value;
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
            if (token.expiresAt < DateTime.Now.AddMinutes(-5)) return true;

            return false;
        }

        private async Task RefreshTokenIfNecessary()
        {
            await Task.CompletedTask;

            if (IsTokenExpired(_authToken) == false) return;

            _authToken = await GetAuthTokenFromCache();
            if (_authToken is not null) return;

            _authToken = await GetAuthToken();
            if (_authToken is null)
                throw new InvalidOperationException("Failed to obtain auth token");

            StorePinIdAndCode(_authToken.id.ToString(), _authToken.code);
        }

        private async Task<PinResponse?> GetAuthToken()
        {
            var pin = await GenerateNewCode();
            if (pin is null)
                throw new InvalidOperationException("Could not generate new pin");

            var loginUrl = BuildLoginUrl(pin);

            _options.SignInUrlCallback?.Invoke(loginUrl);

            return await FetchAuthToken(pin.id.ToString(), pin.code);
        }

        private async Task<PinResponse?> GetAuthTokenFromCache()
        {
            var (id, code) = RetrievePinIdAndCode();

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(code)) return null;

            var token = await GetTokenStatus(id, code);

            if (string.IsNullOrWhiteSpace(token?.authToken))
                return null;

            return token;
        }

        private async Task<PinResponse?> FetchAuthToken(string id, string code)
        {
            var timeoutAt = DateTime.Now.Add(_options.Timeout);

            while (DateTime.Now < timeoutAt)
            {
                var token = await GetTokenStatus(id, code);

                if (string.IsNullOrWhiteSpace(token?.authToken) == false)
                    return token;

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            return null;
        }

        private async Task<PinResponse?> GenerateNewCode()
        {
            await Task.CompletedTask;

            // Create new PIN
            using var newPinMessage = new HttpRequestMessage(HttpMethod.Post, NewPinURL);
            newPinMessage.Headers.Add(PlexHeaders.XPlexProduct, "Plex Playlist Exporter");
            newPinMessage.Headers.Add(PlexHeaders.XPlexClientIdentifier, _options.ClientId);
            newPinMessage.Headers.Add("Accept", "application/json");
            newPinMessage.Headers.Add(PlexHeaders.Strong, "true");

            using var newPinResponse = await _client.SendAsync(newPinMessage);
            var responseString = await newPinResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PinResponse>(responseString);
        }

        private string BuildLoginUrl(PinResponse pin)
        {
            // Build login URL for the browser
            var loginBaseString = "https://app.plex.tv/auth#";
            var clientID = $"clientID={_options.ClientId}";
            var code = $"code={pin.code}";
            var appName = $"{HttpUtility.UrlEncode("context[device][product]")}={HttpUtility.UrlEncode("Plex Web")}";
            var appVersion = $"{HttpUtility.UrlEncode("context[device][version]")}=v1";
            var device = $"{HttpUtility.UrlEncode("context[device][device]")}=Plex";
            var forwardUrl = $"forwardUrl={HttpUtility.UrlEncode("https://app.plex.tv")}";
            var loginURL = $"{loginBaseString}?{clientID}&{appName}&{appVersion}&{device}&{forwardUrl}&{code}";

            return loginURL;
        }

        private void StorePinIdAndCode(string id, string code)
        {
            if (_secretsManager is not null)
            {
                _secretsManager.SetSecret(SecretTokenId, id);
                _secretsManager.SetSecret(SecretTokenCode, code);
            }
        }

        private (string id, string code) RetrievePinIdAndCode()
        {
            if (_secretsManager is null) return default;

            return (_secretsManager.GetSecret(SecretTokenId), _secretsManager.GetSecret(SecretTokenCode));
        }

        private async Task<PinResponse?> GetTokenStatus(string id, string code)
        {
            await Task.CompletedTask;

            var formData = new Dictionary<string, string>
            {
                { "code", code },
                { PlexHeaders.XPlexClientIdentifier, _options.ClientId },
            };

            using var pinStatusMessage = new HttpRequestMessage(HttpMethod.Get, string.Format(PinStatusURL, id));
            using var formContent = new FormUrlEncodedContent(formData);
            pinStatusMessage.Content = formContent;

            pinStatusMessage.Headers.Add("Accept", "application/json");

            using var pinStatusResponse = await _client.SendAsync(pinStatusMessage);
            
            if (pinStatusResponse.IsSuccessStatusCode == false)
                return null;

            var tokenResponseString = await pinStatusResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PinResponse>(tokenResponseString);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                // Dispose managed state (managed objects)
                if (_disposeClient)
                    _client?.Dispose();
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer
            // Set large fields to null
            _disposedValue = true;
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