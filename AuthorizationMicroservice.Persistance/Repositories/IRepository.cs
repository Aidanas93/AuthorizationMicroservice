using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.Persistance.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task InsertRecordAsync(T record);
        Task<List<T>> LoadRecordsAsync();
        Task<T> LoadRecordByIdAsync(Guid id);
        Task UpsertRecordAsync(Guid id, T record);
        Task DeleteRecordAsync(Guid id);
    }
}
