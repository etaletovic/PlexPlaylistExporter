using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using PlexPlaylistExporter.DataProtection.Abstractions.Contracts;

namespace PlexPlaylistExporter.DataProtection.Windows
{
    public class DataProtectionService : IDataProtectionService
    {
        public string Protect(string value)
        {
            if (value is null) return string.Empty;

            byte[] entropy = Encoding.ASCII.GetBytes(Assembly.GetExecutingAssembly().FullName);
            byte[] data = Encoding.ASCII.GetBytes(value);
            return Convert.ToBase64String(ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser));
        }

        public Task<string> ProtectAsync(string value)
        {
            return Task.Run(() => Protect(value));
        }

        public string Unprotect(string value)
        {
            if (value is null) return string.Empty;

            byte[] protectedData = Convert.FromBase64String(value);
            
            var assemblyName = Assembly.GetExecutingAssembly()?.FullName;
            if (string.IsNullOrWhiteSpace(assemblyName))
                throw new InvalidOperationException("Assembly name can't be null or empty");

            byte[] entropy = Encoding.ASCII.GetBytes(assemblyName);
            return Encoding.ASCII.GetString(ProtectedData.Unprotect(protectedData, entropy, DataProtectionScope.CurrentUser));
        }

        public Task<string> UnprotectAsync(string value)
        {
            return Task.Run(() => Unprotect(value));
        }
    }
}
