using System.Net.Http.Json;
using InventorySystem_Shared.Inventory;
using System.Net.Http.Headers;

namespace InventorySystem_MAUI.Service
{
    public class InventoryService
    {
        private readonly HttpClient _httpClient;

        public InventoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIClient");
        }

        public async Task<List<InventoryResponse>> GetItemsByWarehouse(string warehouseId, InventoryQuery query)
        {
            var queryString = BuildQueryString(query);
            var response = await _httpClient.GetAsync($"api/inventory/warehouse/{warehouseId}{queryString}");

            return await response.Content.ReadFromJsonAsync<List<InventoryResponse>>()
                   ?? new List<InventoryResponse>();
        }

        public async Task<InventoryResponse> GetById(string id)
        {
            var response = await _httpClient.GetAsync($"api/inventory/{id}");
            return await response.Content.ReadFromJsonAsync<InventoryResponse>();
        }

        public async Task DeleteById(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/inventory/{id}");
        }

        public async Task<InventoryResponse> CreateItem(string warehouseId, InventoryCreate dto, FileResult photo)
        {
            using var content = new MultipartFormDataContent();


            AddContentFields(content, dto);

            if (photo != null)
            {
                var stream = await photo.OpenReadAsync();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(photo.ContentType);
                content.Add(fileContent, "photo", photo.FileName);
            }

            var response = await _httpClient.PostAsync($"api/inventory/warehouse/{warehouseId}", content);
            return await response.Content.ReadFromJsonAsync<InventoryResponse>();
        }

        private string BuildQueryString(InventoryQuery query)
        {
            var parts = new List<string>
            {
                $"Page={query.Page ?? 1}",
                $"PageSize={query.PageSize ?? 10}"
            };

            if (!string.IsNullOrEmpty(query.Name)) parts.Add($"Name={Uri.EscapeDataString(query.Name)}");
            if (query.InventoryType.HasValue) parts.Add($"InventoryType={query.InventoryType}");
            if (query.MinPrice.HasValue) parts.Add($"MinPrice={query.MinPrice}");
            if (query.MaxPrice.HasValue) parts.Add($"MaxPrice={query.MaxPrice}");

            if (!string.IsNullOrEmpty(query.SortBy))
                parts.Add($"SortBy={query.SortBy}&SortDescending={query.SortDescending}");

            return "?" + string.Join("&", parts);
        }

        private void AddContentFields(MultipartFormDataContent content, object dto)
        {
            foreach (var prop in dto.GetType().GetProperties())
            {
                var value = prop.GetValue(dto);
                if (value != null)
                {
                    if (value is Dictionary<string, string> dict)
                    {
                        foreach (var kvp in dict)
                        {
                            content.Add(new StringContent(kvp.Value), $"CustomFields[{kvp.Key}]");
                        }
                    }
                    else
                    {
                        content.Add(new StringContent(value.ToString()), prop.Name);
                    }
                }
            }
        }
    }
}