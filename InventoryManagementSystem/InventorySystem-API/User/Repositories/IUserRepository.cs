using InventorySystem_API.User.Model;

namespace InventorySystem_API.User.Repositories
{
    public interface IUserRepository
    {
        public Task<UserModel?> GetUserByIdAsync(string id);
        public Task<UserModel?> GetUserByEmailAsync(string email);

        public Task<UserModel> CreateAsync(UserModel userInfo);
        public Task<UserModel> UpdateAsync(UserModel userInfo);
        public Task DeleteAsync(string id);

        public Task<bool> IsExistsAsync(string id);


        public Task<List<UserModel>> GetUserByCompanyId(string companyId);
    }
}
