// See https://aka.ms/new-console-template for more information
using PlexPlaylistExporter.Core.Contracts.Services;

public class WindowsSecretsManager : ISecretsManager
{
    private readonly string _secretsLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                            "PlexExporter",
                                                            ".secrets");
    private readonly IDataProtectionService _dataProtectionService;

    public WindowsSecretsManager(IDataProtectionService dataProtectionService)
    {
        if (Directory.Exists(_secretsLocation) == false)
            Directory.CreateDirectory(_secretsLocation);
        _dataProtectionService = dataProtectionService;
    }

    public string GetSecret(string secretName)
    {
        var secretLocation = Path.Combine(_secretsLocation, secretName);

        if (!File.Exists(secretLocation))
            return string.Empty;

        return _dataProtectionService.Unprotect(File.ReadAllText(secretLocation));
    }

    public void RemoveSecret(string secretName)
    {
        var secretLocation = Path.Combine(_secretsLocation, secretName);

        if (File.Exists(secretLocation))
            File.Delete(secretLocation);
    }

    public void SetSecret(string secretName, string secretValue)
    {
        RemoveSecret(secretName);

        var secretLocation = Path.Combine(_secretsLocation, secretName);
        File.WriteAllText(secretLocation, _dataProtectionService.Protect(secretValue));
    }
}
