using AuthorizationMicroservice.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using AuthorizationMicroservice.Application.Infrastructure.Exceptions;
using System.Net;
using AuthorizationMicroservice.Application.Infrastructure;

namespace AuthorizationMicroservice.Application.CryptographyService
{
    public class JWTHandler : IJWTHandler
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration configuration;
        private JwtSecurityToken decodedToken;

        public JWTHandler(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CreateJWTToken(UserCredential user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration.GetSection("JWTToken:SecretKey").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.UserInfo.Role),
                }),
                Expires = new LocalDateTime().LocalDate.AddMinutes(24 * 60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenCreate = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(tokenCreate);
            return token;
        }
        public bool HasAdminRole(string token)
        {
            DecodeToken(token);
            var claims = decodedToken.Claims.ToList();
            string role = claims.Where(x => x.Type == "role").FirstOrDefault().Value;
            return role == "Admin" || role == "SuperAdmin" ? true : false;
        }

        public bool IsValid(string token)
        {
            if (token == null)
                return false;

            DecodeToken(token);
            var expiresAt = UnixTimeStampToDateTime((double)decodedToken.Payload.Exp);

            int result = DateTime.Compare(expiresAt, new LocalDateTime().LocalDate);
            return result > 0 ? true : false;
        }

        public Guid TokenId(string token)
        {
            DecodeToken(token);
            var claims = decodedToken.Claims.ToList();
            string id = claims.Where(x => x.Type == "nameid").FirstOrDefault().Value;
            Guid guidId;
            if (Guid.TryParse(id, out guidId))
            {
                return guidId;
            }
            else
            {
                throw new StatusCodeException(HttpStatusCode.BadRequest, "Token was not valid");
            }
        }
        public string RemoveBearerFromJWTToken(string token)
        {
            string[] values = token.Split(" ");
            return values[1];
        }


        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private void DecodeToken(string token)
        {
            var stream = token;
            var handler = new JwtSecurityTokenHandler();
            decodedToken = handler.ReadToken(stream) as JwtSecurityToken;
        }
    }
}
