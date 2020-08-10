namespace AuthorizationMicroservice.Application.CryptographyService
{
    public interface IAesHandler
    {
        string Decrypt(string cipherText);
        string Encrypt(string plainText);
    }
}