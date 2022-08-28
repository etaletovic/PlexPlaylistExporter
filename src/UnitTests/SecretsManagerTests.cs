using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PlexPlaylistExporter.DataProtection.Abstractions.Contracts;
using PlexPlaylistExporter.SecretsManager;

namespace UnitTests
{
    [TestClass]
    public class SecretsManagerTests
    {
        [TestMethod]
        public void SecretsManager_CreatesSecretsDirectory()
        {
            if (Directory.Exists(SecretsManager.SecretsLocation))
                Directory.Delete(SecretsManager.SecretsLocation);

            Assert.IsFalse(Directory.Exists(SecretsManager.SecretsLocation));

            _ = new SecretsManager();

            Assert.IsTrue(Directory.Exists(SecretsManager.SecretsLocation));
        }

        [TestMethod]
        public void SecretsManager_IsEncryptionEnabled()
        {
            Assert.IsTrue(new SecretsManager(Mock.Of<IDataProtectionService>()).EncryptionEnabled);
            Assert.IsFalse(new SecretsManager().EncryptionEnabled);
        }

        [TestMethod]
        public void SecretsManager_CanSetSecret()
        {
            var secretKey = "top-secret";
            var secretValue = "top-secret-value-123";
            var secretsManager = new SecretsManager();
            secretsManager.SetSecret(secretKey, secretValue);

            var secretFile = Path.Combine(SecretsManager.SecretsLocation, secretKey);
            Assert.IsTrue(File.Exists(secretFile));

            var contents = File.ReadAllText(secretFile);
            Assert.AreEqual(secretValue, contents);
        }

        [TestMethod]
        public void SecretsManager_CanGetSecret()
        {
            var secretKey = "top-secret";
            var secretValue = "top-secret-value-123";
            var secretsManager = new SecretsManager();
            secretsManager.SetSecret(secretKey, secretValue);

            var storedValue = secretsManager.GetSecret(secretKey);

            Assert.AreEqual(secretValue, storedValue);
        }

        [TestMethod]
        public void SecretsManager_NonExistingSecret_ReturnsEmptyString()
        {
            var secretsManager = new SecretsManager();

            var storedValue = secretsManager.GetSecret("invalid");

            Assert.AreEqual(string.Empty, storedValue);
        }

        [TestMethod]
        public void SecretsManager_InvalidSecretArgument_ThrowsArgumentException()
        {
            var secretsManager = new SecretsManager();
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _ = secretsManager.GetSecret(null);
            });
        }

        [TestMethod]
        public void SecretsManager_CanRemoveSecret()
        {
            var secretKey = "top-secret";
            var secretValue = "top-secret-value-123";
            var secretsManager = new SecretsManager();
            secretsManager.SetSecret(secretKey, secretValue);

            var secretFile = Path.Combine(SecretsManager.SecretsLocation, secretKey);
            Assert.IsTrue(File.Exists(secretFile));

            secretsManager.RemoveSecret(secretKey);

            Assert.IsFalse(File.Exists(secretFile));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (Directory.Exists(SecretsManager.SecretsLocation))
                Directory.Delete(SecretsManager.SecretsLocation, true);
        }
    }
}