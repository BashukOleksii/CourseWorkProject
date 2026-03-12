using InventorySystem_API.Company.Models;

namespace InventorySystem_API.Company.Repository
{
    public interface ICompanyRepository
    {
        Task<CompanyModel?> GetById(string id);
        Task Delete(string id);
        Task<CompanyModel> Create(CompanyModel companyModel);
        Task<CompanyModel> Update(CompanyModel companyModel);
    }
}
