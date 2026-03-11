using InventorySystem_API.User.Model;
using InventorySystem_Shared.User;
using MongoDB.Driver;

namespace InventorySystem_API.User.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly IMongoCollection<UserModel> _mongoCollection;

        public UserRepository(IMongoDatabase mongoDatabase) =>
            _mongoCollection = mongoDatabase.GetCollection<UserModel>("Users");

        public async Task<UserModel> CreateAsync(UserModel userInfo)
        {
            await _mongoCollection.InsertOneAsync(userInfo);
            return userInfo;
        }


        public async Task DeleteAsync(string id) =>
            await _mongoCollection.DeleteOneAsync(user => user.Id == id);
        

        public async Task<List<UserModel>> GetUsersByCompanyId(string companyId) =>
            await _mongoCollection.Find(user => user.CompanyId == companyId).ToListAsync();


        public async Task<UserModel?> GetUserByEmailAsync(string email) =>
            await _mongoCollection.Find(user => user.Email == email).FirstOrDefaultAsync();


        public async Task<UserModel?> GetUserByIdAsync(string id) =>
            await _mongoCollection.Find(user => user.Id == id).FirstOrDefaultAsync();

        public async Task<List<UserModel>> GetUserByRole(UserRole userRole, string companyId) =>
            await _mongoCollection.Find(user => user.CompanyId == companyId && user.UserRole == userRole).ToListAsync();
        

        public async Task<bool> IsExistsAsync(string id) =>
            await _mongoCollection.Find(user => user.Id == id).AnyAsync();


        public async Task<UserModel> UpdateAsync(UserModel userInfo)
        {
            await _mongoCollection.ReplaceOneAsync(user => user.Id == userInfo.Id, userInfo);
            return userInfo;
        }
       
    }
}
