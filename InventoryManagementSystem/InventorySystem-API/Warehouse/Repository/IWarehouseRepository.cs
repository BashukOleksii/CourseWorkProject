using InventorySystem_API.Warehouse.Models;
using MongoDB.Driver;

namespace InventorySystem_API.Warehouse.Repository
{
    public interface IWarehouseRepository
    {
        Task<List<WarehouseModel>> Get(FilterDefinition<WarehouseModel> filter, SortDefinition<WarehouseModel>? sort, int? pageSize, int? page);
        Task<WarehouseModel?> GetById(string id);
        Task<List<WarehouseModel>> GetByIds(string[] id);
        Task<List<string>> GetIdsByCompanyId(string companyId); 

        Task<WarehouseModel> Create(WarehouseModel model);
        Task<WarehouseModel> Update(WarehouseModel model);

        Task Delete(string id);

    }
}
