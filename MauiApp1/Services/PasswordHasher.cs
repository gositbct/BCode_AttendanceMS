using System.Security.Cryptography;
using System.Text;

namespace MauiApp1.Services
{
    // Minimal password hashing helper so credentials are never stored in plain text.
    // Note: for a production app you'd want a per-user salt (e.g. PBKDF2/BCrypt);
    // this keeps things dependency-free while avoiding plaintext passwords.
    public static class PasswordHasher
    {
        public static string Hash(string rawPassword)
        {
            var bytes = Encoding.UTF8.GetBytes(rawPassword ?? string.Empty);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
