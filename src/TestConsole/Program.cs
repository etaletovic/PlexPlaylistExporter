// See https://aka.ms/new-console-template for more information
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Web;

Console.WriteLine("Hello, World!");

const string NewPinURL = "https://plex.tv/api/v2/pins";
const string PinStatusURL = "https://plex.tv/api/v2/pins/{0}";

const string ClientId = "252fa245-1c8a-4cb7-8519-6b004cde7449";


using var client = new HttpClient();

// Create new PIN
using var newPinMessage = new HttpRequestMessage(HttpMethod.Post, NewPinURL);
newPinMessage.Headers.Add("X-Plex-Product", "Plex Playlist Exporter");
newPinMessage.Headers.Add("X-Plex-Client-Identifier", ClientId);
newPinMessage.Headers.Add("Accept", "application/json");
newPinMessage.Headers.Add("strong", "true");


using var newPinResponse = await client.SendAsync(newPinMessage);
var responseString = await newPinResponse.Content.ReadAsStringAsync();
var response = JsonSerializer.Deserialize<PinResponse>(responseString);

// Build login URL for the browser
var loginBaseString =       "https://app.plex.tv/auth#";
var clientID        =       $"clientID={ClientId}";
var code            =       $"code={response.code}";
var appName         =       $"{HttpUtility.UrlEncode("context[device][product]")}={HttpUtility.UrlEncode("Plex Web")}";
var appVersion      =       $"{HttpUtility.UrlEncode("context[device][version]")}=v1";
var device          =       $"{HttpUtility.UrlEncode("context[device][device]")}=Plex";
var forwardUrl      =       $"forwardUrl={HttpUtility.UrlEncode("https://google.com")}";
var loginURL        =       $"{loginBaseString}?{clientID}&{appName}&{appVersion}&{device}&{forwardUrl}&{code}";

// After using login url start get auth code from PIN status
using var pinStatusMessage = new HttpRequestMessage(HttpMethod.Get, string.Format(PinStatusURL, response.id))
{
    Content = new FormUrlEncodedContent(new Dictionary<string, string>
    {
        { "code", response.code },
        { "X-Plex-Client-Identifier", ClientId },
    })
};
pinStatusMessage.Headers.Add("Accept", "application/json");

using var pinStatusResponse = await client.SendAsync(pinStatusMessage);
responseString = await pinStatusResponse.Content.ReadAsStringAsync();
response = JsonSerializer.Deserialize<PinResponse>(responseString);


Console.ReadKey();


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
