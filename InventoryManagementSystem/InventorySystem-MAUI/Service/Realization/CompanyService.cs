using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Helper.Exceptions;
using InventorySystem_Shared.Company;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace InventorySystem_MAUI.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly HttpClient _httpClient;

        public CompanyService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIClient");
        }

        public async Task<string> CreateCompany(CompanyDTO company)
        {
            var response = await _httpClient.PostAsJsonAsync("api/company", company);

            var companyResponse = await response.Content.ReadFromJsonAsync<CompanyResponse>();

            return companyResponse!.Id;
        }

        public async Task<CompanyResponse> GetMyCompany()
        {
            var response = await _httpClient.GetAsync("api/company");
            return await response.Content.ReadFromJsonAsync<CompanyResponse>();
        }

        public async Task<CompanyResponse> UpdateMyCompany(CompanyUpdate update)
        {
            var response = await _httpClient.PatchAsJsonAsync("api/company", update);
            return await response.Content.ReadFromJsonAsync<CompanyResponse>();
        }
    }
}
