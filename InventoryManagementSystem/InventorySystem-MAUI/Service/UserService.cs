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
    }
}
