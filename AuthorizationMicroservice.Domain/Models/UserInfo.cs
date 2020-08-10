using System;

namespace AuthorizationMicroservice.Domain.Models
{
    public class UserInfo
    {
        public UserInfo()
        {
            Role = "User";
        }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime RegistrationTime { get; set; }
        public string Role { get; set; }
        public string PictureUrl { get; set; }
    }
}
