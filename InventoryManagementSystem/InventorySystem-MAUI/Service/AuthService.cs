using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Helper.Exceptions;
using InventorySystem_Shared.User;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace InventorySystem_MAUI.Service
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly UserContextService _userContextService;

        public AuthService(IHttpClientFactory httpClientFactory, UserContextService userContextService)
        {
            _httpClient = httpClientFactory.CreateClient("APIClient");
            _userContextService = userContextService;
        }

        

        public async Task Login(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/sign-in", new UserLogin { Email = email, Password = password });

            if (!response.IsSuccessStatusCode)
                await ApiException.ShowException(response);

            var tokens = await response.Content.ReadFromJsonAsync<TokensDataResponse>();

            var user = await _httpClient.GetFromJsonAsync<UserResponse>("api/user/whoami");

            await _userContextService.SetUserContextAsync(
                   user,
                   tokens.AccessToken,
                   tokens.RefreshToken
            );
        }

        public async Task Register(UserRegister userRegister)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/sign-up", userRegister);

            if(!response.IsSuccessStatusCode)
                await ApiException.ShowException(response);
        }
    }
}
