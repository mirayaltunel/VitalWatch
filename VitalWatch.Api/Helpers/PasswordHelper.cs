namespace VitalWatch.Api.Helpers
{
    using System.Security.Cryptography;

    using System.Security.Cryptography;

    using System.Security.Cryptography;

    public static class PasswordHelper
    {
        private const int KeySize = 32;
        private const int Iterations = 100_000;
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

        // Password ve Salt'ı string alıp, sonucu Hex string döner
        public static string GetHash(string password, string saltHex)
        {
            byte[] salt = Convert.FromHexString(saltHex);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithm,
                KeySize);

            return Convert.ToHexString(hash);
        }

        // Yeni kayıtlar için rastgele Hex string formatında Salt üretir
        public static string GenerateSalt(int size = 16)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(size);
            return Convert.ToHexString(salt);
        }
    }
}
