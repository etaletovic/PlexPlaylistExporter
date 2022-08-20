// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Web;
using System.Xml.Serialization;

Console.WriteLine("Hello, World!");

const string AccountUrl = "https://plex.tv/users/account";

using var client = new HttpClient();
var plexClient = new PlexClient(client, Options.Create(new PlexClientOptions { ClientId = "252fa245-1c8a-4cb7-8519-6b004cde7449" }));


await plexClient.AuthenticateClient((url) =>
{
    using var process = Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
});


var servers = await plexClient.GetServersAsync();
var resources = await plexClient.GetResourcesAsync();

var serversToDisplay = resources.Where(r => r.Provides.Equals("server",StringComparison.OrdinalIgnoreCase)).ToList();

Console.ReadKey();
class PlexClientOptions
{
    public string ClientId { get; set; }
}

class PlexClient
{
    const string ServersUrl = "https://plex.tv/pms/servers.xml";
    const string DevicesUrl = "https://plex.tv/devices.xml";
    const string NewPinURL = "https://plex.tv/api/v2/pins";
    const string PinStatusURL = "https://plex.tv/api/v2/pins/{0}";
    const string ResourcesURL = "https://plex.tv/api/resources?includeHttps={0}&includeRelay={1}";

    private readonly HttpClient _client;
    private readonly PlexClientOptions _options;
    private PinResponse? _authToken;

    public PlexClient(HttpClient client, IOptions<PlexClientOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task AuthenticateClient(Action<string> promptUrlSignInCallback, TimeSpan timeout = default)
    {
        await Task.CompletedTask;

        if (promptUrlSignInCallback == null)
            throw new ArgumentNullException(nameof(promptUrlSignInCallback));

        if (timeout == default)
            timeout = TimeSpan.FromSeconds(30);

        _authToken = null;

        // Create new PIN
        using var newPinMessage = new HttpRequestMessage(HttpMethod.Post, NewPinURL);
        newPinMessage.Headers.Add(PlexHeaders.XPlexProduct, "Plex Playlist Exporter");
        newPinMessage.Headers.Add(PlexHeaders.XPlexClientIdentifier, _options.ClientId);
        newPinMessage.Headers.Add("Accept", "application/json");
        newPinMessage.Headers.Add(PlexHeaders.Strong, "true");

        using var newPinResponse = await _client.SendAsync(newPinMessage);
        var responseString = await newPinResponse.Content.ReadAsStringAsync();
        var pin = JsonSerializer.Deserialize<PinResponse>(responseString);

        if (pin is null)
            throw new InvalidOperationException("Could not generate PIN to claim");

        // Build login URL for the browser
        var loginBaseString = "https://app.plex.tv/auth#";
        var clientID = $"clientID={_options.ClientId}";
        var code = $"code={pin.code}";
        var appName = $"{HttpUtility.UrlEncode("context[device][product]")}={HttpUtility.UrlEncode("Plex Web")}";
        var appVersion = $"{HttpUtility.UrlEncode("context[device][version]")}=v1";
        var device = $"{HttpUtility.UrlEncode("context[device][device]")}=Plex";
        var forwardUrl = $"forwardUrl={HttpUtility.UrlEncode("https://app.plex.tv")}";
        var loginURL = $"{loginBaseString}?{clientID}&{appName}&{appVersion}&{device}&{forwardUrl}&{code}";

        promptUrlSignInCallback.Invoke(loginURL);

        var timeoutAt = DateTime.Now.Add(timeout);
        var formData = new Dictionary<string, string>
        {
            { "code", pin.code },
            { PlexHeaders.XPlexClientIdentifier, _options.ClientId },
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

        if (_authToken is null)
            throw new InvalidOperationException("Failed to obtain auth token");
    }

    public async Task<IEnumerable<Device>> GetResourcesAsync(bool includeHttps = true, bool includeRelay = true)
    {
        await Task.CompletedTask;

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, string.Format(ResourcesURL, includeHttps.ToInt(), includeRelay.ToInt()));
        requestMessage.Headers.Add(PlexHeaders.XPlexToken, _authToken?.authToken);

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
        requestMessage.Headers.Add(PlexHeaders.XPlexToken, _authToken?.authToken);

        using var responseMessage = await _client.SendAsync(requestMessage);

        var responseStream = await responseMessage.Content.ReadAsStreamAsync();

        var serializer = new XmlSerializer(typeof(MediaContainer));
        if (serializer.Deserialize(responseStream) is not MediaContainer mediaContainer)
            return Enumerable.Empty<Server>();

        return mediaContainer.Servers ?? Enumerable.Empty<Server>();
    }
}

public class Resource
{

}

public class MediaContainer
{
    [XmlElement("Server")]
    public List<Server> Servers { get; set; }

    [XmlElement("Device")]
    public List<Device> Devices { get; set; }

    [XmlAttribute("machineIdentifier")]
    public string MachineIdentifier { get; set; }

    [XmlAttribute("identifier")]
    public string Identifier { get; set; }
    
    [XmlAttribute("friendlyName")]
    public string FriendlyName { get; set; }
}

public class Device
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("product")]
    public string Product { get; set; }

    [XmlAttribute("productVersion")]
    public string ProductVersion { get; set; }

    [XmlAttribute("platform")]
    public string Platform { get; set; }

    [XmlAttribute("platformVersion")]
    public string PlatformVersion { get; set; }

    [XmlAttribute("device")]
    public string DeviceString { get; set; }

    [XmlAttribute("createdAt")]
    public long CreatedAt { get; set; }

    [XmlAttribute("lastSeenAt")]
    public long LastSeenAt { get; set; }

    [XmlAttribute("owned")]
    public int Owned { get; set; }

    [XmlAttribute("synced")]
    public int Synced { get; set; }

    [XmlAttribute("clientIdentifier")]
    public string ClientIdentifier { get; set; }

    [XmlAttribute("provides")]
    public string Provides { get; set; }

    [XmlAttribute("accessToken")]
    public string AccessToken { get; set; }

    [XmlAttribute("publicAddress")]
    public string PublicAddress { get; set; }

    [XmlAttribute("httpsRequired")]
    public int HttpsRequired { get; set; }

    [XmlAttribute("relay")]
    public int Relay { get; set; }

    [XmlAttribute("dnsRebindingProtection")]
    public int DnsRebindingProtection { get; set; }
    
    [XmlAttribute("natLoopbackSupported")]
    public int NatLoopbackSupported { get; set; }
    
    [XmlAttribute("publicAddressMatches")]
    public int PublicAddressMatches { get; set; }

    [XmlAttribute("presence")]
    public int Presence { get; set; }

    [XmlElement("Connection")]
    public List<Connection> Connections { get; set; }
}

public class Connection
{
    [XmlAttribute("protocol")]
    public string Protocol { get; set; }

    [XmlAttribute("address")]
    public string Address { get; set; }

    [XmlAttribute("port")]
    public int Port { get; set; }

    [XmlAttribute("uri")]
    public string Uri { get; set; }

    [XmlAttribute("local")]
    public int Local { get; set; }

    [XmlAttribute("relay")]
    public int Relay { get; set; }
}

public class Server
{
    [XmlAttribute("accessToken")]
    public string AccessToken { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("address")]
    public string Address { get; set; }
    
    [XmlAttribute("port")]
    public int Port { get; set; }

    [XmlAttribute("version")]
    public string Version { get; set; }

    [XmlAttribute("scheme")]
    public string Scheme { get; set; }

    [XmlAttribute("host")]
    public string Host { get; set; }

    [XmlAttribute("localAddresses")]
    public string LocalAddresses { get; set; }

    [XmlAttribute("machineIdentifier")]
    public string MachineIdentifier { get; set; }

    [XmlAttribute("createdAt")]
    public long CreatedAt { get; set; }

    [XmlAttribute("updatedAt")]
    public long UpdatedAt { get; set; }

    [XmlAttribute("owned")]
    public int Owned { get; set; }

    [XmlAttribute("synced")]
    public int Synced { get; set; }
}


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

public class Location
{
    public string code { get; set; }
    public bool european_union_member { get; set; }
    public string continent_code { get; set; }
    public string country { get; set; }
    public string city { get; set; }
    public string time_zone { get; set; }
    public string postal_code { get; set; }
    public bool in_privacy_restricted_country { get; set; }
    public string subdivisions { get; set; }
    public string coordinates { get; set; }
}

public static class BoolExtensios
{
    public static int ToInt(this bool value) => value ? 1 : 0;
    public static long ToLong(this bool value) => value ? 1 : 0;
    public static short ToShort(this bool value) => value ? (short)1 : (short)0;
}