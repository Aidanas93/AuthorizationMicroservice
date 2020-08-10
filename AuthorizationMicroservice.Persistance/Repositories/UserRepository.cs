using AuthorizationMicroservice.Domain.Models;
using AuthorizationMicroservice.Persistance.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.Persistance.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext<UserCredential> context;
        public UserRepository()
        {
            context = new ApplicationDbContext<UserCredential>();
        }

        public async Task DeleteRecordAsync(Guid id)
        {
            var filter = Builders<UserCredential>.Filter.Eq("Id", id);
            await context.Collection.DeleteOneAsync(filter);
        }

        public async Task InsertRecordAsync(UserCredential record)
        {

            context
                .Collection
                .Indexes
                .CreateOne(
                    new CreateIndexModel<UserCredential>(
                        new IndexKeysDefinitionBuilder<UserCredential>().Ascending(x => x.Email),
                        new CreateIndexOptions { Unique = true }));
            await context.Collection.InsertOneAsync(record);
        }

        public async Task<UserCredential> LoadRecordByEmailAsync(string email)
        {
            var filter = Builders<UserCredential>.Filter.Eq("Email", email);
            return await context.Collection.Find(filter).FirstAsync();
        }

        public async Task<UserCredential> LoadRecordByIdAsync(Guid id)
        {
            var filter = Builders<UserCredential>.Filter.Eq("Id", id);
            return await context.Collection.Find(filter).FirstAsync();
        }

        public async Task<List<UserCredential>> LoadRecordsAsync()
        {
            return await context.Collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpsertRecordAsync(Guid id, UserCredential record)
        {
            var result = await context.Collection.ReplaceOneAsync(
                new BsonDocument("_id", id),
                record,
                new ReplaceOptions { IsUpsert = true });
        }
    }
}
