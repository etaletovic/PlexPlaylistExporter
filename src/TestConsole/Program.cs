// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Options;
using PlexPlaylistExported.PlexClient;
using PlexPlaylistExported.PlexClient.Authentication;
using PlexPlaylistExporter.Core.Services;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Threading;
using System.Web;
using System.Xml.Serialization;

Console.WriteLine("Hello, World!");

const string AccountUrl = "https://plex.tv/users/account";

var secretsManager = new WindowsSecretsManager(new DataProtectionService());
var clientId = "252fa245-1c8a-4cb7-8519-6b004cde7449";
using var client = new HttpClient();
var plexClient = new PlexClient(client, new OAuthAuthentication(clientId, AuthenticateClient));

var servers = await plexClient.GetServersAsync();
var resources = await plexClient.GetResourcesAsync();

var serversToDisplay = resources.Where(r => r.Provides.Equals("server", StringComparison.OrdinalIgnoreCase)).ToList();

Console.ReadKey();

static void AuthenticateClient(string url)
{
    using var process = Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
}

public class Resource
{

}
