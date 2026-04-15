using InventorySystem_MAUI.Helper;
using InventorySystem_Shared.User;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace InventorySystem_MAUI.Service
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly UserContextService _userContext;

        public UserService(
            IHttpClientFactory httpClientFactory,
            UserContextService userContext)
        {
            _httpClient = httpClientFactory.CreateClient("APIClient");
            _userContext = userContext;
        }

        public async Task<UserResponse> UpdateUser(string id, UserUpdate update, FileResult photo = null)
        {
            using var content = new MultipartFormDataContent();

            if (!string.IsNullOrEmpty(update.Name)) content.Add(new StringContent(update.Name), "Name");
            if (!string.IsNullOrEmpty(update.Email)) content.Add(new StringContent(update.Email), "Email");
            if (!string.IsNullOrEmpty(update.Password)) content.Add(new StringContent(update.Password), "Password");

            if (photo != null)
            {
                var stream = await photo.OpenReadAsync();
                content.Add(new StreamContent(stream), "photo", photo.FileName);
            }

            var response = await _httpClient.PatchAsync($"api/user/{id}", content);
            var updated = await response.Content.ReadFromJsonAsync<UserResponse>();

            await _userContext.SetUserContextAsync(
                updated,
                _userContext.AccessToken,
                _userContext.RefreshToken);

            return updated;
        }

        public async Task<List<UserResponse>> GetUsers(UserQuery query)
        {
            var queryString = $"?Page={query.Page}&PageSize={query.PageSize}";

            if (!string.IsNullOrEmpty(query.Name))
                queryString += $"&Name={query.Name}";
            if (!string.IsNullOrEmpty(query.WarehouseId))
                queryString += $"&WarehouseId={query.WarehouseId}";
            if (query.UserRole.HasValue)
                queryString += $"&UserRole={query.UserRole}";
            if (!string.IsNullOrEmpty(query.SortBy))
                queryString += $"&SortBy={query.SortBy}&SortDescending={query.SortDescending}";

            var response = await _httpClient.GetAsync($"api/user{queryString}");
            return await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        }

        public async Task DeleteUser(string id) =>
            await _httpClient.DeleteAsync($"api/user/{id}");

        public async Task UpdateUserWarehouses(string userId, List<string> warehouseIds) =>
             await _httpClient.PatchAsJsonAsync($"api/user/{userId}/warehouses", warehouseIds);


        public async Task<UserResponse> GetUserById(string id)
        {
            var response = await _httpClient.GetAsync($"api/user/{id}");
            return await response.Content.ReadFromJsonAsync<UserResponse>();
        }

        public async Task<byte[]> GetUserReport(UserQuery query)
        {
            var queryString = $"?Name={query.Name}&WarehouseId={query.WarehouseId}&UserRole={query.UserRole}&SortBy={query.SortBy}&SortDescending={query.SortDescending}";
            var response = await _httpClient.GetAsync($"api/export/user-report{queryString}");
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
