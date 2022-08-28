public interface ISecretsManager
{
    string GetSecret(string secretName);
    void SetSecret(string secretName, string secretValue);
    void RemoveSecret(string secretName);
}
