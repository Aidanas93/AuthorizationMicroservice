using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AuthorizationMicroservice.Application.CryptographyService
{
    public class AesHandler : IAesHandler
    {
        private readonly byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private readonly int BlockSize = 128;
        private readonly IConfiguration configuration;

        public AesHandler(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Encrypt(string plainText)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(plainText);
            SymmetricAlgorithm crypt = Aes.Create();
            HashAlgorithm hash = MD5.Create();
            crypt.BlockSize = BlockSize;
            crypt.Key = Encoding.Unicode.GetBytes(configuration.GetSection("AES:SecretKey").Value);
            crypt.IV = IV;


            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream =
                   new CryptoStream(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                }
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public string Decrypt(string cipherText)
        {
            string plainText = null;
            byte[] bytes = Convert.FromBase64String(cipherText);
            SymmetricAlgorithm crypt = Aes.Create();
            HashAlgorithm hash = MD5.Create();
            crypt.Key = Encoding.Unicode.GetBytes(configuration.GetSection("AES:SecretKey").Value);
            crypt.IV = IV;

            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                using (CryptoStream cryptoStream =
                   new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] decryptedBytes = new byte[bytes.Length];
                    int read = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                    plainText = Encoding.Unicode.GetString(decryptedBytes, 0, read);
                }

                return plainText;
            }
        }
    }
}
