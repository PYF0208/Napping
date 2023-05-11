using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Napping_PJ.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: System.Text.Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }

        public static bool VerifyPassword(string password, string salt, string hash)
            => HashPassword(password, salt) == hash;

        public static string GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }
    }
}
