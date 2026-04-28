using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.View;
using InventorySystem_Shared.User;

namespace InventorySystem_MAUI.ViewModel
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly IUserContextService _userContextService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        public LoginViewModel(IAuthService authService, IUserContextService userContextService)
        {
            _authService = authService;
            _userContextService = userContextService;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await ShellService.DisplayAlert("Помилка", "Заповніть всі поля", "OK");
                return;
            }

            await RunBusyTask(async () =>
            {
                await _authService.Login(Email, Password);

                if(_userContextService.CurrentUser.UserRole == UserRole.manager)
                    await ShellService.AbsoluteOpenPage(nameof(ManagerWarehousePage));
                else
                    await ShellService.AbsoluteOpenPage(nameof(UserListPage));

            });
        }

        [RelayCommand]
        private async Task GoToRegister()
        {
            await ShellService.NavigateTo(nameof(CompanyCreatePage));
        }

        [RelayCommand]
        private async Task ForgotPassword()
        {
            await ShellService.NavigateTo(nameof(ForgotPasswordPage));
        }
    }
}