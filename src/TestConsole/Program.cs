using Microsoft.Extensions.Options;
using PlexPlaylistExporter.DataProtection.Windows;
using PlexPlaylistExporter.PlexClient;
using PlexPlaylistExporter.PlexClient.Authentication;
using PlexPlaylistExporter.SecretsManager;
using System.Diagnostics;

Console.WriteLine("Hello, World!");

var secretsManager = new SecretsManager(new DataProtectionService());
var clientId = "252fa245-1c8a-4cb7-8519-6b004cde7449";
using var client = new HttpClient();

var oAuthOptions = Options.Create(new OAuthAuthenticationOptions
{
    ClientId = clientId,
    SignInUrlCallback = AuthenticateClient,
});

using var plexClient = new PlexClient(client, new OAuthAuthentication(oAuthOptions, new SecretsManager()));

var servers = await plexClient.GetServersAsync();
var resources = await plexClient.GetResourcesAsync();

var serversToDisplay = resources.Where(r => r.Provides.Equals("server", StringComparison.OrdinalIgnoreCase)).ToList();

Console.ReadKey();

static void AuthenticateClient(string url)
{
    using var process = Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
}
