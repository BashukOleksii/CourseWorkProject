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

        public async Task<UserModel> Create(UserModel userInfo)
        {
            await _mongoCollection.InsertOneAsync(userInfo);
            return userInfo;
        }


        public async Task Delete(string id) =>
            await _mongoCollection.DeleteOneAsync(user => user.Id == id);

        public async Task<List<UserModel>> Get(FilterDefinition<UserModel> filter, int pageSize, int page) =>
            await _mongoCollection
            .Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
        

        public async Task<List<UserModel>> GetByCompanyId(string companyId) =>
            await _mongoCollection.Find(user => user.CompanyId == companyId).ToListAsync();


        public async Task<UserModel?> GetByEmail(string email) =>
            await _mongoCollection.Find(user => user.Email == email).FirstOrDefaultAsync();


        public async Task<UserModel?> GetById(string id) =>
            await _mongoCollection.Find(user => user.Id == id).FirstOrDefaultAsync();

        public async Task<List<UserModel>> GetByRole(UserRole userRole, string companyId) =>
            await _mongoCollection.Find(user => user.CompanyId == companyId && user.UserRole == userRole).ToListAsync();
        

        public async Task<UserModel> Update(UserModel userInfo)
        {
            await _mongoCollection.ReplaceOneAsync(user => user.Id == userInfo.Id, userInfo);
            return userInfo;
        }
       
    }
}
