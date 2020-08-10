using AuthorizationMicroservice.Domain.Models;
using System;

namespace AuthorizationMicroservice.Application.CryptographyService
{
    public interface IJWTHandler
    {
        string CreateJWTToken(UserCredential user);
        bool IsValid(string token);
        string RemoveBearerFromJWTToken(string token);
        Guid TokenId(string token);
        bool HasAdminRole(string token);
    }
}
