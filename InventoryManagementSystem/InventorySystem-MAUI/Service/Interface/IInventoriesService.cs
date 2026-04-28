using System.Net.Http.Json;
using InventorySystem_Shared.Inventory;
using System.Net.Http.Headers;

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


    }
}