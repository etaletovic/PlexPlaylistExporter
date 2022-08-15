using PlexPlaylistExporter.Contracts;

namespace PlexPlaylistExporter.Services
{
    public class ShellNavigationService : INavigationService
    {
        public async Task GoTo(string route, IDictionary<string, object> args)
        {
            await Task.CompletedTask;

            var absoluteRoute = $"//{route}";

            if (args is null)
                await Shell.Current?.GoToAsync(absoluteRoute);
            else
                await Shell.Current?.GoToAsync(absoluteRoute, args);
        }
    }

}
