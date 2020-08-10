using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AuthorizationMicroservice.Domain.Models
{
    public class UserCredential
    {
        public UserCredential()
        {
            AccessToken = new AccessToken();
        }

        [BsonId]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public AccessToken AccessToken { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}
