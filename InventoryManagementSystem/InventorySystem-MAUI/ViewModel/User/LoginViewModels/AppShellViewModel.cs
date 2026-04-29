using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.View;
using InventorySystem_Shared.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_MAUI.ViewModel
{
    public partial class AppShellViewModel : ObservableObject
    {
        private readonly IUserContextService _userContext;

        public AppShellViewModel(IUserContextService userContext)
        {
            _userContext = userContext;
            userContext.UserContextChanged += UpdateAuthenticationState;
            UpdateAuthenticationState();
        }

        [ObservableProperty] public bool isAuthenticated;
        [ObservableProperty] public bool isNotAuthenticated;
        [ObservableProperty] public bool isAdmin;
        [ObservableProperty] public bool isManager;
        [ObservableProperty] public FlyoutBehavior flyoutBehavior;
        [ObservableProperty] private string userPhoto;

        private void UpdateAuthenticationState()
        {
            IsAuthenticated = _userContext.AccessToken is not null;
            IsNotAuthenticated = _userContext.AccessToken is null;
            IsAdmin = _userContext.CurrentUser?.UserRole == UserRole.admin;
            IsManager = _userContext.CurrentUser?.UserRole == UserRole.manager;
            FlyoutBehavior = IsAuthenticated ? FlyoutBehavior.Flyout : FlyoutBehavior.Disabled;
            UserPhoto = Conection.BaseURI + _userContext.CurrentUser?.PhotoURI ;
        }


        [RelayCommand]
        async Task Logout()
        {
            _userContext.LogOut();
            UpdateAuthenticationState();
            await ShellService.AbsoluteOpenPage(nameof(WelcomePage));
        }

        [RelayCommand]
        private async Task GoToProfile()
        {
            await ShellService.NavigateTo(nameof(ProfilePage));
        }
    }
}
