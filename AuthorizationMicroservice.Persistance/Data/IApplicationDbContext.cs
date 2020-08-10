using MongoDB.Driver;

namespace AuthorizationMicroservice.Persistance.Data
{
    public interface IApplicationDbContext<T> where T : class
    {
        IMongoCollection<T> Collection { get; }
    }
}
