using System.Xml.Serialization;
using PlexPlaylistExporter.Core.Extensions;
using PlexPlaylistExporter.PlexClient.Contracts.Interfaces;
using PlexPlaylistExporter.PlexClient.Contracts.Models;

namespace PlexPlaylistExporter.PlexClient
{
    public class PlexClient: IDisposable
    {
        const string ServersUrl = "https://plex.tv/pms/servers.xml";
        const string ResourcesURL = "https://plex.tv/api/resources?includeHttps={0}&includeRelay={1}";

        private readonly HttpClient _client;
        private readonly IPlexAuthentication _plexAuthentication;
       
        private bool _shouldDisposeClient;
        private bool _disposedValue;

        public PlexClient(IPlexAuthentication plexAuthentication)
            : this(new HttpClient(), plexAuthentication)
        {
            _shouldDisposeClient = true;
        }

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

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)

                    if(_shouldDisposeClient)
                        _client.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _disposedValue = true;
            }
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PlexClient()
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