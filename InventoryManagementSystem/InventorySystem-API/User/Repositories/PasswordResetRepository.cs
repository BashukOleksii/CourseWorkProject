using InventorySystem_API.User.Models;
using MongoDB.Driver;

namespace InventorySystem_API.User.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly IMongoCollection<ResetPasswordModel> _collection;
                
        public PasswordResetRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<ResetPasswordModel>("password_resets");

            var indexKeys = Builders<ResetPasswordModel>.IndexKeys.Ascending(x => x.CreatedAt);

            var indexOptions = new CreateIndexOptions
            {
                ExpireAfter = TimeSpan.FromHours(1)
            };

            var indexModel = new CreateIndexModel<ResetPasswordModel>(indexKeys, indexOptions);

            _collection.Indexes.CreateOne(indexModel);
        }

        public async Task Create(ResetPasswordModel resetPassword) =>
            await _collection.InsertOneAsync(resetPassword);


        public async Task Delete(string email) =>
            await _collection.DeleteOneAsync(x => x.Email == email);

        public async Task<ResetPasswordModel?> Get(string email) =>
            await _collection.Find(x => x.Email == email).FirstOrDefaultAsync();

    }
}
