using System;
using System.Text;
using System.Security.Cryptography;

namespace BTL.Helpers
{
    public static class HashHelper
    {
        public static string CreateSaltKey(int size)
        {
            using (var provider = RandomNumberGenerator.Create())
            {
                var buff = new byte[size];
                provider.GetBytes(buff);
                return Convert.ToBase64String(buff);
            }
        }
        public static string CreateHash(byte[] data, string hashAlgorithm)
        {
            if (string.IsNullOrEmpty(hashAlgorithm))
                throw new ArgumentNullException(nameof(hashAlgorithm));

            var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashAlgorithm);
            if (algorithm == null)
                throw new Exception("Unrecognized hash name");

            return BitConverter.ToString(algorithm.ComputeHash(data)).Replace("-", string.Empty);
        }
        public static string CreatePasswordHash(string password, string saltKey, string hashAlgorithm)
        {
            return CreateHash(Encoding.UTF8.GetBytes(string.Concat(password, saltKey)), hashAlgorithm);
        }
    }
}