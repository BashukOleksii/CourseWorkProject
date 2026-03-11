using InventorySystem_Shared.User;

namespace InventorySystem_API.User.Services
{
    public interface IUserService
    {
        Task<UserResponse> GetUserById(string id, string companyIdClient);
        Task<List<UserResponse>> GetUsers(string companyIdClient);
        Task<UserResponse> UpdateUser(string userId, UserUpdate userUpdate, string companyIdClient);
        Task DeleteUser(string id, string companyIdClient);

        Task AddWarehouses(string userId, string[] warehouses, string companyIdClient);
        Task RemoveWarehouses(string userId, string[] warehouses, string companyIdClient);
    }
}
