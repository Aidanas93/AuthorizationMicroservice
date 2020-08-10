using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Text;

namespace AuthorizationMicroservice.Application.CryptographyService
{
    public static class HashHandler
    {
        public static string Create(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: value,
                                salt: Encoding.UTF8.GetBytes(salt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 10000,
                                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }
    }
}
