using InventorySystem_Shared.Warehouse;

namespace InventorySystem_API.Warehouse.Service
{
    public interface IWarehouseService
    {
        Task<List<WarehouseResponse>> Get(string companyId, WarehouseQuery? warehouseQuery);
        Task<WarehouseResponse> GetById(string warehouseId, string companyId);
        Task<List<WarehouseResponse>> GetByIds(string[] id, string companyId);
        Task<List<string>> GetIdsByCompanyId(string companyId);

        Task<WarehouseResponse> Create(WarehouseDTO model, string companyId);
        Task<WarehouseResponse> Update(string id, WarehouseUpdate update, string companyId);

        Task Delete(string id, string companyId);
    }
}
