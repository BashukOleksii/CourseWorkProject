using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.View;

namespace InventorySystem_MAUI.ViewModel
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlertAsync("Помилка", "Заповніть всі поля", "OK");
                return;
            }

            await RunBusyTask(async () =>
            {
                await _authService.Login(Email, Password);

                await Shell.Current.GoToAsync($"///{nameof(WarehouseListPage)}");

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