using InventorySystem_API.Loging.Models;
using InventorySystem_Shared.Loging;
using MongoDB.Driver;

namespace InventorySystem_API.Loging.Service
{
    public interface ILogService
    {
        Task<List<AuditLogResponse>> Get(AuditLogQuery auditLogQuery, string companyId);
        Task<AuditLogResponse> GetById(string id);
        Task<AuditLogResponse> Create(AuditLogCreate dto);
        Task Delete(string id);
    }
}
