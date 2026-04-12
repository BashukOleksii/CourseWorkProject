using InventorySystem_Shared.Warehouse;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace InventorySystem_MAUI.Service
{
    public class WarehouseService
    {
        private readonly HttpClient _httpClient;

        public WarehouseService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIClient");
        }

        public async Task<List<WarehouseResponse>> GetWarehouses(WarehouseQuery query)
        {
            var queryString = $"?Page={query.Page}&PageSize={query.PageSize}";
            
            if (!string.IsNullOrEmpty(query.Name)) 
                queryString += $"&Name={query.Name}";
            if (!string.IsNullOrEmpty(query.PartDescription))
                queryString += $"&PartDescription={query.PartDescription}";
            if (query.Address is not null)
            {
                if(!string.IsNullOrEmpty(query.Address.Country))
                    queryString += $"&Address.Country={query.Address.Country}";
                if (!string.IsNullOrEmpty(query.Address.State))
                    queryString += $"&Address.State={query.Address.State}";
                if (!string.IsNullOrEmpty(query.Address.District))
                    queryString += $"&Address.District={query.Address.District}";
                if (!string.IsNullOrEmpty(query.Address.City))
                    queryString += $"&Address.City={query.Address.City}";
                if (!string.IsNullOrEmpty(query.Address.Street))
                    queryString += $"&Address.Street={query.Address.Street}";
                if (!string.IsNullOrEmpty(query.Address.HouseNumber))
                    queryString += $"&Address.HouseNumber={query.Address.HouseNumber}";
            }

            if (query.MinArea.HasValue)
                queryString += $"&MinArea={query.MinArea.Value}";
            if (query.MaxArea.HasValue)
                queryString += $"&MaxArea={query.MaxArea.Value}";

            if(!string.IsNullOrEmpty(query.SortBy))
                queryString += $"&SortBy={query.SortBy}&OrderByDescending={query.OrderByDescending}";

            var response = await _httpClient.GetAsync($"api/warehouse{queryString}");

            return await response.Content.ReadFromJsonAsync<List<WarehouseResponse>>();
        }
    }
}
