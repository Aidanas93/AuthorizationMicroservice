using AuthorizationMicroservice.Application.CryptographyService;
using AuthorizationMicroservice.Domain.Models;
using AuthorizationMicroservice.Persistance.Repositories;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.API.Infrastructure
{
    public class SuperAdminHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public SuperAdminHandler(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task LoadSuperAdminAsync()
        {
            await _userRepository.LoadRecordByEmailAsync(_configuration.GetSection("SuperAdmin:Email").Value);
        }

        public async Task CreateSuperAdminAsync()
        {
            string salt = SaltHandler.Create();
            UserCredential user = new UserCredential()
            {
                Email = _configuration.GetSection("SuperAdmin:Email").Value,
                PasswordSalt = salt,
                PasswordHash = HashHandler.Create(_configuration.GetSection("SuperAdmin:Password").Value, salt),
                AccessToken = new AccessToken()
                {
                    JWTToken = null
                },
                UserInfo = new UserInfo()
                {
                    Firstname = "Firstname",
                    Lastname = "Lastname",
                    Role = "SuperAdmin",
                }
            };
            await _userRepository.InsertRecordAsync(user);
        }
    }
}
