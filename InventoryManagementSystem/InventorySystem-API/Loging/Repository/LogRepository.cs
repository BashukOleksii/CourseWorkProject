using InventorySystem_API.Loging.Models;
using MongoDB.Driver;

namespace InventorySystem_API.Loging.Repository
{
    public class LogRepository : ILogRepository
    {
        private readonly IMongoCollection<AuditLogModel> _collection;

        public LogRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<AuditLogModel>("AuditLogs");
        }

        public async Task<AuditLogModel> Create(AuditLogModel model)
        {
            await _collection.InsertOneAsync(model);
            return model;
        }

        public async Task Delete(string id) =>
            await _collection.DeleteOneAsync(id);


        public async Task<List<AuditLogModel>> Get(FilterDefinition<AuditLogModel> filter, int pageSize, int page) =>
            await _collection
            .Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();


        public async Task<AuditLogModel?> GetById(string id) =>
            await _collection.Find(auditLog => auditLog.Id == id).FirstOrDefaultAsync();
        
    }
}
