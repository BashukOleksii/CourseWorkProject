using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Company;
using InventorySystem_Shared.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_MAUI.ViewModel
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly CompanyService _companyService;
        private readonly UserContextService _userContext;

        [ObservableProperty] private UserResponse currentUser;
        [ObservableProperty] private CompanyResponse company;
        [ObservableProperty] private bool isEditMode;

        [ObservableProperty] private string editName;
        [ObservableProperty] private string editEmail;
        [ObservableProperty] private string newPassword;
        [ObservableProperty] private string confirmPassword;

        public ProfileViewModel(UserService userService, CompanyService companyService, UserContextService userContext)
        {
            _userService = userService;
            _companyService = companyService;
            _userContext = userContext;

            LoadData();
        }

        private async void LoadData()
        {
            await RunBusyTask(async () =>
            {
                CurrentUser = _userContext.CurrentUser;
                EditName = CurrentUser.Name;
                EditEmail = CurrentUser.Email;
                Company = await _companyService.GetMyCompany();
            });
        }

        [RelayCommand]
        private void ToggleEditMode() => IsEditMode = !IsEditMode;

        [RelayCommand]
        private async Task SaveChanges()
        {
            if (!string.IsNullOrEmpty(NewPassword) && NewPassword != ConfirmPassword)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Паролі не збігаються", "OK");
                return;
            }

            await RunBusyTask(async () =>
            {
                var update = new UserUpdate
                {
                    Name = EditName,
                    Email = EditEmail,
                    Password = string.IsNullOrEmpty(NewPassword) ? null : NewPassword
                };

                var updatedUser = await _userService.UpdateUser(CurrentUser.Id, update);

                await _userContext.SetUserContextAsync(
                    updatedUser,
                    _userContext.AccessToken!,
                    _userContext.RefreshToken!);

                CurrentUser = updatedUser;
                IsEditMode = false;
                NewPassword = ConfirmPassword = string.Empty;

            });
        }
    }
}
