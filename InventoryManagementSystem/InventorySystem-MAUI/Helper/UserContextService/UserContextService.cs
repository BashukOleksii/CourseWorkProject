using InventorySystem_Shared.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_MAUI.Helper
{
    public class UserContextService : IUserContextService
    {
        public UserResponse? CurrentUser { get; private set; }
        public string? AccessToken { get; private set; }
        public string? RefreshToken { get; private set; }

        public event Action? UserContextChanged;
            
        public async Task SetUserContextAsync(UserResponse user, string accessToken, string refreshToken)
        {
            CurrentUser = user;
            AccessToken = accessToken;
            RefreshToken = refreshToken;

            string userData = System.Text.Json.JsonSerializer.Serialize(user);

            await SecureStorage.SetAsync("CurrentUser", userData);
            await SecureStorage.SetAsync("AccessToken", accessToken);
            await SecureStorage.SetAsync("RefreshToken", refreshToken);

            UserContextChanged?.Invoke();
        }

        public async Task LoadUserContextAsync()
        {
            try
            {
                string userData = await SecureStorage.GetAsync("CurrentUser");
                AccessToken = await SecureStorage.GetAsync("AccessToken");
                RefreshToken = await SecureStorage.GetAsync("RefreshToken");

                if (!string.IsNullOrEmpty(userData))
                {
                    CurrentUser = System.Text.Json.JsonSerializer.Deserialize<UserResponse>(userData);
                }
                UserContextChanged?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання інформації із Secret Store: {ex.Message}");
            }
        }

        public void LogOut()
        {
            CurrentUser = null;
            AccessToken = RefreshToken = null;
            SecureStorage.Default.RemoveAll();
            UserContextChanged?.Invoke();
        }
    }
}
