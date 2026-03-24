using InventorySystem_API.Inventory.Models;
using MongoDB.Driver;

namespace InventorySystem_API.Inventory.Repository
{
    public interface IInventoryRepository
    {
        Task<List<InventoryModel>> Get(FilterDefinition<InventoryModel> filter, int pageSize, int page);
        Task<InventoryModel?> GetById(string Id);

        Task<InventoryModel> Create(InventoryModel model);
        Task<InventoryModel> Update(InventoryModel model);

        Task DeleteById(string id);
        Task DeleteByWarehouse(string warehouseId);

    }
}
