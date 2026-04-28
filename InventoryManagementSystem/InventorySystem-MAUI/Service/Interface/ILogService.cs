using InventorySystem_Shared.Loging;
using System.Net.Http.Json;

namespace InventorySystem_MAUI.Service
{
    public interface ILogService
    {
        Task<List<AuditLogResponse>> GetLogs(AuditLogQuery query);
    }
}