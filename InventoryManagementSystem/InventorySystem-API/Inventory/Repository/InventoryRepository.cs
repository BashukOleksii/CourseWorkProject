using InventorySystem_API.Inventory.Models;
using InventorySystem_API.Warehouse.Repository;
using MongoDB.Driver;

namespace InventorySystem_API.Inventory.Repository
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IMongoCollection<InventoryModel> _collection;

        public InventoryRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<InventoryModel>("Inventories");
        }

        public async Task<InventoryModel> Create(InventoryModel model)
        {
            await _collection.InsertOneAsync(model);
            return model;
        }

        public async Task DeleteById(string id) =>
            await _collection.DeleteOneAsync(inventory => inventory.Id == id);


        public async Task DeleteByWarehouseId(string warehouseId) =>
            await _collection.DeleteManyAsync(warehouse => warehouse.Id == warehouseId);
        

        public async Task<List<InventoryModel>> Get(FilterDefinition<InventoryModel> filter, int pageSize, int page) =>
            await _collection
            .Find(filter)
            .Skip((page - 1)*pageSize)
            .Limit(pageSize)
            .ToListAsync();


        public async Task<InventoryModel?> GetById(string id) =>
            await _collection.Find(inventory => inventory.Id == id).FirstOrDefaultAsync();

        public async Task<List<InventoryModel>> GetByWarehouseId(string warehouseId) =>
            await _collection.Find(inventory => inventory.WarehouseId == warehouseId).ToListAsync();
        
        public async Task<InventoryModel> Update(InventoryModel model)
        {
            await _collection.ReplaceOneAsync(inventory => inventory.Id == model.Id, model);
            return model;
        }
    }
}
