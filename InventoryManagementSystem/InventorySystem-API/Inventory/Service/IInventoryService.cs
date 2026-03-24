using InventorySystem_API.Inventory.Models;
using InventorySystem_Shared.Inventory;
using MongoDB.Driver;

namespace InventorySystem_API.Inventory.Service
{
    public interface IInventoryService
    {
        Task<List<InventoryResponse>> Get(InventoryQuery? inventoryQuery, string warehouseId);
        Task<InventoryResponse> GetById(string id);

        Task<InventoryResponse> Create(InventoryCreate dto, string warehouseId, IFormFile? photo);
        Task<InventoryResponse> Update(string id, InventoryUpdate dto, IFormFile? photo);

        Task DeleteById(string id);
        Task DeleteByWarehouseId(string warehouseId);
    }
}
