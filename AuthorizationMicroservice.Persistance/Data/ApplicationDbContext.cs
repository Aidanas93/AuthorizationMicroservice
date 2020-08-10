using AuthorizationMicroservice.Persistance.Models;
using MongoDB.Driver;

namespace AuthorizationMicroservice.Persistance.Data
{
    public class ApplicationDbContext<T> : IApplicationDbContext<T> where T : class
    {
        private readonly IMongoDatabase _database = null;
        private readonly string _table;
        public ApplicationDbContext()
        {
            var client = new MongoClient();
            _table = "Users";
            if (client != null)
                _database = client.GetDatabase("AuthorizationMicroservice");
        }

        public IMongoCollection<T> Collection
        {
            get
            {
                return _database.GetCollection<T>(_table);
            }
        }
    }
}
