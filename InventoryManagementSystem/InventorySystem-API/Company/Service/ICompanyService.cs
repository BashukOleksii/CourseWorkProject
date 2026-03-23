using InventorySystem_Shared.Company;

namespace InventorySystem_API.Company.Service
{
    public interface ICompanyService
    {
        Task<CompanyResponse> GetById(string companyId);
        Task<CompanyResponse> Create(CompanyDTO companyDTO);
        Task<CompanyResponse> Update(string companyId,CompanyUpdate companyUpdate);
        Task Delete(string companyId);
    }
}
