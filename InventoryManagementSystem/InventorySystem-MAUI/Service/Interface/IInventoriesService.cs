using InventorySystem_Shared.Company;
using InventorySystem_Shared.Inventory;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace InventorySystem_MAUI.Service
{
    public interface IInventoryService
    {
        Task<List<InventoryResponse>> GetItemsByWarehouse(string warehouseId, InventoryQuery query);

        Task<InventoryResponse> GetById(string id);

        Task DeleteById(string id);

        Task<InventoryResponse> CreateItem(string warehouseId, InventoryCreate dto, FileResult photo);

        Task<InventoryResponse> UpdateItem(string itemId, InventoryUpdate dto, FileResult photo);

        Task<byte[]> GetInventoryReport(string warehouseId, InventoryQuery query);

        Task<byte[]> ExportData(string warehouseId, string format);
        Task ImportData(string warehouseId, FileResult file);
        Task<byte[]> GetSalesReport(string warehouseId, string[] inventoryIds, CompanyDTO provider);
    }
}