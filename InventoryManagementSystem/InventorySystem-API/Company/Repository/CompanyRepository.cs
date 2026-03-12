using InventorySystem_API.Company.Models;
using MongoDB.Driver;

namespace InventorySystem_API.Company.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IMongoCollection<CompanyModel> _collection;

        public CompanyRepository(IMongoDatabase mongoDatabase) =>
            _collection = mongoDatabase.GetCollection<CompanyModel>("Companies");

        public async Task<CompanyModel> Create(CompanyModel companyModel) { 
            await _collection.InsertOneAsync(companyModel);
            return companyModel;
        }


        public async Task Delete(string id) =>
            await _collection.DeleteOneAsync(company => company.Id == id);


        public async Task<CompanyModel?> GetById(string id) =>
            await _collection.Find(company => company.Id == id).FirstOrDefaultAsync();



        public async Task<CompanyModel> Update(CompanyModel companyModel)
        {
            await _collection.ReplaceOneAsync(company => company.Id == companyModel.Id, companyModel);
            return companyModel;
        }

    }
}
