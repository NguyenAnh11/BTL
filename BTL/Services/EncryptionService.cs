using System;
using System.Text;
using System.Security.Cryptography;

namespace BTL.Services
{
    public class EncryptionService : IEncryptionService
    {
        private string CreateHash(byte[] data, string hashAlgorithm)
        {
            if (string.IsNullOrEmpty(hashAlgorithm))
                throw new ArgumentNullException(nameof(hashAlgorithm));

            var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashAlgorithm);
            if (algorithm == null)
                throw new Exception("Unrecognized hash name");

            return BitConverter.ToString(algorithm.ComputeHash(data)).Replace("-", string.Empty);
        }

        public string CreatePasswordHash(string password, string saltKey, string hashAlgorithm)
        {
            return CreateHash(Encoding.UTF8.GetBytes(string.Concat(password, saltKey)), hashAlgorithm);
        }

        public string CreateSaltKey(int size)
        {   
            if(size == 0)   
                throw new ArgumentNullException(nameof(size));

            using (var provider = RandomNumberGenerator.Create())
            {
                var buff = new byte[size];
                provider.GetBytes(buff);
                return Convert.ToBase64String(buff);
            }
        }
    }
}