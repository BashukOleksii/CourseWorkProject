using InventorySystem_API.User.Model;
using InventorySystem_Shared.User;

namespace InventorySystem_API.User.Repositories
{
    public interface IUserRepository
    {
        public Task<UserModel?> GetById(string id);
        public Task<UserModel?> GetByEmail(string email);
        public Task<List<UserModel>> GetByCompanyId(string companyId);
        public Task<List<UserModel>> GetByRole(UserRole userRole, string companyId);


        public Task<UserModel> Create(UserModel userInfo);
        public Task<UserModel> Update(UserModel userInfo);
        public Task Delete(string id);

    }
}
