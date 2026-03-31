using InventorySystem_Shared.User;

namespace InventorySystem_API.User.Services
{
    public interface IUserService
    {
        Task<UserResponse> GetById(string id, string companyIdClient);
        Task<List<UserResponse>> Get(string companyIdClient, UserQuery? userQuery);
        Task<UserResponse> Update(string userId, UserUpdate userUpdate, string companyIdClient, IFormFile? formFile);
        Task Delete(string id, string companyIdClient);

        public Task<long> GetCountInWarehouse(string warehouseId);
        Task AddWarehouses(string userId, string[] warehouses, string companyIdClient);
        Task RemoveWarehouses(string userId, string[] warehouses, string companyIdClient);

        Task AddWarehouseToAdmins(string warehouseId,string companyId);
        Task RemoveWarehouse(string warehouseId, string companyId);
    }
}
