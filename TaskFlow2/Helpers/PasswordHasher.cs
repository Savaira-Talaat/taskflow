using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace TaskFlow2.Helpers
{
    public static class PasswordHasher
    {
        // Format: {iterations}.{saltBase64}.{hashBase64}
        public static string Hash(string password, int iterations = 100_000)
        {
            var salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            var subkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterations, 32);

            return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(subkey)}";
        }

        public static bool Verify(string password, string hashed)
        {
            try
            {
                var parts = hashed.Split('.');
                if (parts.Length != 3) return false;

                var iterations = int.Parse(parts[0]);
                var salt = Convert.FromBase64String(parts[1]);
                var expected = Convert.FromBase64String(parts[2]);

                var actual = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterations, expected.Length);
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch
            {
                return false;
            }
        }
    }
}