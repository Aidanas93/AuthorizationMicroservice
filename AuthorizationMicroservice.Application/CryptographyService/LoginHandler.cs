namespace AuthorizationMicroservice.Application.CryptographyService
{
    public class LoginHandler
    {
        public static bool ValidateUser(string value, string salt, string hash)
        {
            var loginHash = HashHandler.Create(value, salt);

            return loginHash == hash;
        }
    }
}
