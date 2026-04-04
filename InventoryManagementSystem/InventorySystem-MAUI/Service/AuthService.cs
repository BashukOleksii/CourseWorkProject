using InventorySystem_MAUI.Helper;
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

        public AuthService(HttpClient httpClient, UserContextService userContextService)
        {
            _httpClient = httpClient;
            _userContextService = userContextService;
        }

        public async Task Login(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/sign-in", new UserLogin { Email = email, Password = password });

            if (!response.IsSuccessStatusCode)
            {
                var 
            }

        }
    }
}
