using InventorySystem_API.Warehouse.Models;

namespace InventorySystem_API.Warehouse.Repository
{
    public interface IWarehouseRepository
    {
        Task<WarehouseModel?> GeById(string id);
        Task<List<WarehouseModel>?> GetByIds(string[] id);

        Task<WarehouseModel> Create(WarehouseModel model);
        Task<WarehouseModel> Update(WarehouseModel model);

        Task Delete(string id);

    }
}
