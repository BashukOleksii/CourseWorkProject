using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Helper.Exceptions;
using InventorySystem_Shared.Company;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace InventorySystem_MAUI.Service
{
    public interface ICompanyService
    {
        Task<string> CreateCompany(CompanyDTO company);
        Task<CompanyResponse> GetMyCompany();
        Task<CompanyResponse> UpdateMyCompany(CompanyUpdate update);
    }

}
