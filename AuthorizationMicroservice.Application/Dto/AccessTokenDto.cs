using System;

namespace AuthorizationMicroservice.Application.Dto
{
    public class AccessTokenDto
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
    }
}
