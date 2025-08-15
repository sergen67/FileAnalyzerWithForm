using System.Security.Cryptography;

namespace FileAnalyzerWithForm.Auth
{
    public static class PasswordHasher
    {
        public static void Create(string password, out byte[] hash, out byte[] salt, out int iterations)
        {
            iterations = 100_000;
            salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            hash = pbkdf2.GetBytes(32);
        }

        public static bool Verify(string password, byte[] expectedHash, byte[] salt, int iterations)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(expectedHash.Length);
            return CryptographicOperations.FixedTimeEquals(hash, expectedHash);
        }
    }
}
