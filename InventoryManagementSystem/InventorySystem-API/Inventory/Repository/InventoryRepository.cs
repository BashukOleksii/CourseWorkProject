using InventorySystem_API.Inventory.Models;
using InventorySystem_API.Warehouse.Repository;
using Microsoft.IdentityModel.Abstractions;
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

        public async Task CreateMany(List<InventoryModel> inventoryModels) =>
            await _collection.InsertManyAsync(inventoryModels);


        public async Task DeleteById(string id) =>
            await _collection.DeleteOneAsync(inventory => inventory.Id == id);

        public async Task DeleteByIds(string[] ids)
        {
            var filter = Builders<InventoryModel>.Filter.In(inventory => inventory.Id, ids);
            await _collection.DeleteManyAsync(filter);
        }

        public async Task DeleteByWarehouseId(string warehouseId) =>
            await _collection.DeleteManyAsync(warehouse => warehouse.Id == warehouseId);


        public async Task<List<InventoryModel>> Get(
            FilterDefinition<InventoryModel> filter,
            SortDefinition<InventoryModel>? sort,
            int? pageSize,
            int? page)
        {
            var query = _collection.Find(filter);

            if (sort is not null)
                query = query.Sort(sort);

            if (pageSize is not null)
                query = query.Skip((page - 1 ?? 0) * pageSize.Value).Limit(pageSize.Value);

            return await query.ToListAsync();
        }


        public async Task<InventoryModel?> GetById(string id) =>
            await _collection.Find(inventory => inventory.Id == id).FirstOrDefaultAsync();

        public async Task<List<InventoryModel>> GetByIds(string[] ids)
        {
            var filter = Builders<InventoryModel>.Filter.In(inventory => inventory.Id, ids);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<InventoryModel>> GetByWarehouseId(string warehouseId) =>
            await _collection.Find(inventory => inventory.WarehouseId == warehouseId).ToListAsync();

        public async Task<InventoryModel> Update(InventoryModel model)
        {
            await _collection.ReplaceOneAsync(inventory => inventory.Id == model.Id, model);
            return model;
        }

        public async Task UpdateMany(List<InventoryModel> models) 
        {
            var tasks = models.Select(model =>
                _collection.ReplaceOneAsync(inventory => inventory.Id == model.Id, model));
        await Task.WhenAll(tasks);
        }

    }
}
