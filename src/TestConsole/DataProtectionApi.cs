using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace TestConsole
{
    public static class DataProtectionApi
    {
        public static string Protect(this string str)
        {
            if (str is null) return string.Empty;

            byte[] entropy = Encoding.ASCII.GetBytes(Assembly.GetExecutingAssembly().FullName);
            byte[] data = Encoding.ASCII.GetBytes(str);
            return Convert.ToBase64String(ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser));
        }

        public static string Unprotect(this string str)
        {
            if (str is null) return string.Empty;

            byte[] protectedData = Convert.FromBase64String(str);
            byte[] entropy = Encoding.ASCII.GetBytes(Assembly.GetExecutingAssembly().FullName);
            return Encoding.ASCII.GetString(ProtectedData.Unprotect(protectedData, entropy, DataProtectionScope.CurrentUser));
        }
    }
}
