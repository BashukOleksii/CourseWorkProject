using InventorySystem_API.Warehouse.Models;
using MongoDB.Driver;

namespace InventorySystem_API.Warehouse.Repository
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly IMongoCollection<WarehouseModel> _collection;

        public WarehouseRepository(IMongoDatabase database) =>
            _collection = database.GetCollection<WarehouseModel>("Warehouses");

        public async Task<WarehouseModel> Create(WarehouseModel model)
        {
            await _collection.InsertOneAsync(model);
            return model;
        }

        public async Task Delete(string id) =>
            await _collection.DeleteOneAsync(warehouse => warehouse.Id == id);


        public async Task<WarehouseModel?> GetById(string id) =>
            await _collection.Find(warehouse => warehouse.Id == id).FirstOrDefaultAsync();

        public async Task<List<string>> GetIdsByCompanyId(string companyId) =>
            await _collection.Find(warehouse => warehouse.CompanyId == companyId)
            .Project(warehouse => warehouse.Id).ToListAsync();
        
        public async Task<List<WarehouseModel>> GetByIds(string[] ids) =>
            await _collection.Find(warehouse => ids.Contains(warehouse.Id)).ToListAsync();
        
        public async Task<WarehouseModel> Update(WarehouseModel model)
        {
            await _collection.ReplaceOneAsync(warehouse => warehouse.Id == model.Id,model);
            return model;
        }

        public async Task<List<WarehouseModel>> Get(FilterDefinition<WarehouseModel> filter, SortDefinition<WarehouseModel> sort, int pageSize, int page) =>
            await _collection
            .Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .Sort(sort)
            .ToListAsync();        
    }
}
