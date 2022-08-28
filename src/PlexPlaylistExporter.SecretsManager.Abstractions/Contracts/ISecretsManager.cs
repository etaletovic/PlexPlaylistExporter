namespace PlexPlaylistExporter.SecretsManager.Abstractions.Contracts
{
    public interface ISecretsManager
    {
        /// <summary>
        /// Returns secret value
        /// </summary>
        /// <param name="secretName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="SecretsManagerException"/>
        string GetSecret(string secretName);

        /// <summary>
        /// Sets secret value
        /// </summary>
        /// <param name="secretName"></param>
        /// <param name="secretValue"></param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="SecretsManagerException"/>
        void SetSecret(string secretName, string secretValue);

        /// <summary>
        /// Removes secret value
        /// </summary>
        /// <param name="secretName"></param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="SecretsManagerException"/>
        void RemoveSecret(string secretName);
    }
}