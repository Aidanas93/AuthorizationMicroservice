using AuthorizationMicroservice.Domain.Models;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AuthorizationMicroservice.Application.Dto
{
    public class UserCredentialDto
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public AccessToken AccessToken { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}
