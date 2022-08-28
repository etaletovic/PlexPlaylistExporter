using PlexPlaylistExporter.DataProtection.Abstractions.Contracts;
using PlexPlaylistExporter.SecretsManager.Abstractions.Contracts;

namespace PlexPlaylistExporter.SecretsManager
{
    public class SecretsManager : ISecretsManager
    {
        public static readonly string SecretsLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                                "PlexExporter",
                                                                ".secrets");

        private readonly IDataProtectionService _dataProtectionService;

        public SecretsManager(IDataProtectionService dataProtectionService = null)
        {
            if (Directory.Exists(SecretsLocation) == false)
                Directory.CreateDirectory(SecretsLocation);

            _dataProtectionService = dataProtectionService;
            EncryptionEnabled = _dataProtectionService != null;
        }

        public bool EncryptionEnabled { get; private set; }

        public string GetSecret(string secretName)
        {
            if (string.IsNullOrWhiteSpace(secretName))
                throw new ArgumentException($"'{nameof(secretName)}' cannot be null or whitespace.", nameof(secretName));

            try
            {
                var secretLocation = Path.Combine(SecretsLocation, secretName);

                if (!File.Exists(secretLocation))
                    return string.Empty;

                var data = File.ReadAllText(secretLocation);

                return EncryptionEnabled ? _dataProtectionService.Unprotect(data) : data;
            }
            catch (Exception e)
            {
                throw new SecretsManagerException("Could not get the secret", e);
            }
        }

        public void RemoveSecret(string secretName)
        {
            if (string.IsNullOrWhiteSpace(secretName))
                throw new ArgumentException($"'{nameof(secretName)}' cannot be null or whitespace.", nameof(secretName));

            try
            {
                var secretLocation = Path.Combine(SecretsLocation, secretName);

                if (File.Exists(secretLocation))
                    File.Delete(secretLocation);
            }
            catch (Exception e)
            {
                throw new SecretsManagerException("Could not remove the secret", e);
            }
        }

        public void SetSecret(string secretName, string secretValue)
        {
            if (string.IsNullOrWhiteSpace(secretName))
                throw new ArgumentException($"'{nameof(secretName)}' cannot be null or whitespace.", nameof(secretName));

            try
            {
                RemoveSecret(secretName);

                var secretLocation = Path.Combine(SecretsLocation, secretName);

                var data = EncryptionEnabled ? _dataProtectionService.Protect(secretValue) : secretValue;

                File.WriteAllText(secretLocation, data);
            }
            catch (Exception e)
            {
                throw new SecretsManagerException("Could not set the secret", e);
            }
        }
    }
}