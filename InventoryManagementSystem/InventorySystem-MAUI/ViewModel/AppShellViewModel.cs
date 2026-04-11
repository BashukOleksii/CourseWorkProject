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
        private readonly UserContextService _userContext;

        public AppShellViewModel(UserContextService userContext) =>
            _userContext = userContext;
        

        public bool IsAuthenticated => _userContext.IsAuthenticated;
        public bool IsNotAuthenticated => !_userContext.IsAuthenticated;

        public bool IsAdmin => _userContext.CurrentUser.UserRole == UserRole.admin;
        public bool IsManager => _userContext.CurrentUser.UserRole == UserRole.manager;

        public bool IsManagerOrAdmin => IsAdmin || IsManager;

        public FlyoutBehavior FlyoutBehavior =>
            IsAuthenticated ? FlyoutBehavior.Flyout : FlyoutBehavior.Disabled;

        [ObservableProperty]
        private string userPhoto;

        public void UpdateHeader() =>
            UserPhoto = Conection.BaseURI + _userContext.CurrentUser.PhotoURI.TrimStart('/');
        

        [RelayCommand]
        async Task Logout()
        {
            _userContext.LogOut();
            OnPropertyChanged(nameof(IsAuthenticated));
            OnPropertyChanged(nameof(IsNotAuthenticated));
            OnPropertyChanged(nameof(FlyoutBehavior));
            await ShellService.AbsoluteOpenPage(nameof(WelcomePage));
        }

        [RelayCommand]
        private async Task GoToProfile()
        {
            // Перехід до профілю користувача
        }
    }
}
