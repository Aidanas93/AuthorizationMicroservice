using AuthorizationMicroservice.Domain.Models;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.Persistance.Repositories
{
    public interface IUserRepository : IRepository<UserCredential>
    {
        Task<UserCredential> LoadRecordByEmailAsync(string email);
    }
}
