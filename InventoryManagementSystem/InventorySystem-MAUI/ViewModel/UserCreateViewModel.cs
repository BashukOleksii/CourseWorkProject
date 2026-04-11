using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.View;
using InventorySystem_Shared.User;

namespace InventorySystem_MAUI.ViewModel
{
    [QueryProperty(nameof(CompanyId), "CompanyId")] 
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty] private string companyId;
        [ObservableProperty] private string name;
        [ObservableProperty] private string email;
        [ObservableProperty] private string password;
        [ObservableProperty] private FileResult? selectedPhoto;
        [ObservableProperty] private ImageSource? previewImage = "default_user.png";

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task PickPhoto()
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo != null)
                {
                    SelectedPhoto = photo;
                    var stream = await photo.OpenReadAsync();
                    PreviewImage = ImageSource.FromStream(() => stream);
                }
            }
            catch
            {
                await Shell.Current.DisplayAlertAsync("Помилка", "Не вдалося вибрати фото", "OK");
            }
        }

        [RelayCommand]
        private async Task Register()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Заповніть усі обов'язкові поля", "OK");
                return;
            }

            await RunBusyTask(async () =>
            {
                var userRegister = new UserRegister
                {
                    CompanyId = CompanyId,
                    Name = Name,
                    Email = Email,
                    Password = Password,
                    UserRole = UserRole.admin
                };

                await _authService.Register(userRegister, SelectedPhoto);

                await Shell.Current.DisplayAlertAsync("Success", "Користувача зареєстровано!", "OK");
                await ShellService.NavigateTo(nameof(LoginPage));

            });
        }
    }
}