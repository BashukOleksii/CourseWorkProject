using InventorySystem_Shared.Inventory;
using InventorySystem_Shared.Loging;
using InventorySystem_Shared.User;
using InventorySystem_Shared.Warehouse;

namespace InventorySystem_API.Report.Service
{
    public interface IReportService
    {
        Task<byte[]> GetInventoryReport(InventoryQuery? inventoryQuery, string warehouseId);
        Task<byte[]> GetWarehouseReport(WarehouseQuery? warehouseQuery, string companyId);
        Task<byte[]> GetUserReport(UserQuery? userQuery, string companyId);
    }
}
