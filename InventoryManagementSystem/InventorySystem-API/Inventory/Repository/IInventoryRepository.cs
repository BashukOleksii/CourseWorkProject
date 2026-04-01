using InventorySystem_API.Inventory.Models;
using MongoDB.Driver;

namespace InventorySystem_API.Inventory.Repository
{
    public interface IInventoryRepository
    {
        Task<List<InventoryModel>> Get(FilterDefinition<InventoryModel> filter,
            SortDefinition<InventoryModel>? sort,
            int? pageSize,
            int? page);

        Task<InventoryModel?> GetById(string id);
        Task<List<InventoryModel>> GetByWarehouseId(string warehouseId);

        Task<InventoryModel> Create(InventoryModel model);
        Task<InventoryModel> Update(InventoryModel model);

        Task DeleteById(string id);
        Task DeleteByWarehouseId(string warehouseId);

        Task CreateMany(List<InventoryModel> inventoryModels);
    }
}
