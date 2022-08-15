namespace PlexPlaylistExporter;

public partial class App : Application
{
	public App(AppShell shell)
	{
		if (shell is null)
		{
			throw new ArgumentNullException(nameof(shell));
		}

		InitializeComponent();

		MainPage = shell;
	}
}
