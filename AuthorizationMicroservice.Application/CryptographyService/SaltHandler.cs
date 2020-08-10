using System;
using System.Security.Cryptography;

namespace AuthorizationMicroservice.Application.CryptographyService
{
    public class SaltHandler
    {
        public static string Create()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
    }
}
