using System.Xml.Serialization;
using PlexPlaylistExporter.Core.Extensions;
using PlexPlaylistExporter.PlexClient.Contracts.Interfaces;
using PlexPlaylistExporter.PlexClient.Contracts.Models;

namespace PlexPlaylistExporter.PlexClient
{
    public class PlexClient
    {
        const string ServersUrl = "https://plex.tv/pms/servers.xml";
        const string DevicesUrl = "https://plex.tv/devices.xml";
        const string NewPinURL = "https://plex.tv/api/v2/pins";
        const string PinStatusURL = "https://plex.tv/api/v2/pins/{0}";
        const string ResourcesURL = "https://plex.tv/api/resources?includeHttps={0}&includeRelay={1}";

        private readonly HttpClient _client;
        private readonly IPlexAuthentication _plexAuthentication;

        public PlexClient(HttpClient client, IPlexAuthentication plexAuthentication)
        {
            _client = client;
            _plexAuthentication = plexAuthentication;
        }

        public async Task<IEnumerable<Device>> GetResourcesAsync(bool includeHttps = true, bool includeRelay = true)
        {
            await Task.CompletedTask;

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, string.Format(ResourcesURL, includeHttps.ToInt(), includeRelay.ToInt()));

            foreach (var authHeader in await _plexAuthentication.GetHeaders())
                requestMessage.Headers.Add(authHeader.Key, authHeader.Value);

            using var responseMessage = await _client.SendAsync(requestMessage);

            var responseStream = await responseMessage.Content.ReadAsStreamAsync();

            var serializer = new XmlSerializer(typeof(MediaContainer));
            if (serializer.Deserialize(responseStream) is not MediaContainer mediaContainer)
                return Enumerable.Empty<Device>();

            return mediaContainer.Devices ?? Enumerable.Empty<Device>();
        }

        public async Task<IEnumerable<Server>> GetServersAsync()
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, ServersUrl);

            foreach (var authHeader in await _plexAuthentication.GetHeaders())
                requestMessage.Headers.Add(authHeader.Key, authHeader.Value);

            using var responseMessage = await _client.SendAsync(requestMessage);

            var responseStream = await responseMessage.Content.ReadAsStreamAsync();

            var serializer = new XmlSerializer(typeof(MediaContainer));
            if (serializer.Deserialize(responseStream) is not MediaContainer mediaContainer)
                return Enumerable.Empty<Server>();

            return mediaContainer.Servers ?? Enumerable.Empty<Server>();
        }
    }

}