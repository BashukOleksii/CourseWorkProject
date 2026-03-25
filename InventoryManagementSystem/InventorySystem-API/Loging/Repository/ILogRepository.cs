using InventorySystem_API.Loging.Models;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace InventorySystem_API.Loging.Repository
{
    public interface ILogRepository
    {
        Task<List<AuditLogModel>> Get(FilterDefinition<AuditLogModel> filter, int pageSize, int page);
        Task<AuditLogModel?> GetById(string id);
        Task<AuditLogModel> Create(AuditLogModel model);
        Task Delete(string id);
    }
}
