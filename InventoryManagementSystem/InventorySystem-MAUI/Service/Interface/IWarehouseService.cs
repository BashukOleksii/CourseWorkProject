using InventorySystem_Shared.Warehouse;
using System.Net.Http.Json;

namespace InventorySystem_MAUI.Service
{
    public interface IWarehouseService
    {
        Task<List<WarehouseResponse>> GetWarehouses(WarehouseQuery query);
        Task DeleteWarehouse(string id);
        Task<WarehouseResponse> UpdateWarehouse(string id, WarehouseUpdate update);
        Task<WarehouseResponse> CreateWarehouse(WarehouseDTO dto);
        Task<WarehouseResponse> GetWarehouseById(string id);
        Task<byte[]> GetWarehouseReport(WarehouseQuery query);
        Task<List<WarehouseResponse>> GetWarehousesByIds(List<string> ids);

    }
}
