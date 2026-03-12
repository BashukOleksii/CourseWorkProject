using AutoMapper;
using InventorySystem_API.Company.Models;
using InventorySystem_Shared.Company;

namespace InventorySystem_API.Company.Mapper
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile() 
        {
            CreateMap<CompanyDTO, CompanyModel>();
            CreateMap<CompanyModel, CompanyResponse>();
        }
    }
}
