using InventorySystem_Shared.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_MAUI.Helper
{
    public interface IUserContextService
    {
        UserResponse? CurrentUser { get;}
        string? AccessToken { get; }
        string? RefreshToken { get; }

        event Action? UserContextChanged;
        Task SetUserContextAsync(UserResponse user, string accessToken, string refreshToken);
        Task LoadUserContextAsync();        
        void LogOut();

    }
}
