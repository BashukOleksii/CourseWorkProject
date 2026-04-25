using CommunityToolkit.Mvvm.ComponentModel; 
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;

namespace InventorySystem_MAUI.ViewModel
{
    public partial class ForgotPasswordViewModel : BaseViewModel
    {
        private readonly ResetPasswordService _resetPasswordService;

        [ObservableProperty] private string email;
        [ObservableProperty] private string resetCode;
        [ObservableProperty] private string newPassword;
        [ObservableProperty] private bool isCodeSent; 

        public ForgotPasswordViewModel(ResetPasswordService resetPasswordService)
        {
            _resetPasswordService = resetPasswordService;
            IsCodeSent = false;
        }

        [RelayCommand]
        private async Task SendCode()
        {
            if (string.IsNullOrWhiteSpace(Email)) return;

            await RunBusyTask(async () =>
            {
                await _resetPasswordService.RequestResetCode(Email);
                IsCodeSent = true;
                await Shell.Current.DisplayAlertAsync("Успіх", "Код відправлено на пошту", "OK");
            });

        }

        [RelayCommand]
        private async Task ResetPassword()
        {
            if (string.IsNullOrWhiteSpace(ResetCode) || string.IsNullOrWhiteSpace(NewPassword)) return;

            await RunBusyTask(async () =>
            {
                await _resetPasswordService.ConfirmResetPassword(Email, ResetCode, NewPassword);
                await ShellService.GoBack();
            });

        }

        [RelayCommand]
        private async Task GoBack() => await ShellService.GoBack();
    }
}