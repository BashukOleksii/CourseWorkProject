using InventorySystem_Shared.User;

namespace InventorySystem_API.User.Services
{
    public interface IUserService
    {
        Task<UserResponse> GetById(string id, string companyIdClient);
        Task<List<UserResponse>> Get(string companyIdClient, UserQuery? userQuery, string? userId);
        Task<UserResponse> Update(string userId, UserUpdate userUpdate, string companyIdClient, IFormFile? formFile);
        Task Delete(string id, string companyIdClient);

        public Task<long> GetCountInWarehouse(string warehouseId);
        Task UpdateWarehouses(string userId, string[] warehouses, string companyIdClient);
        Task RemoveWarehouse(string warehouseId, string companyId);
    }
}
