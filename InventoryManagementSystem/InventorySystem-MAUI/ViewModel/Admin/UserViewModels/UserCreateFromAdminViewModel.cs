using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.View;
using InventorySystem_Shared.User;

namespace InventorySystem_MAUI.ViewModel
{
    [QueryProperty(nameof(SelectedWarehouseIds), "SelectedWarehouseIds")]
    public partial class UserCreateFromAdminViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly UserContextService _userContext;

        [ObservableProperty] private string name;
        [ObservableProperty] private string email;
        [ObservableProperty] private string password;

        [ObservableProperty] private FileResult? photo;
        [ObservableProperty] private ImageSource? previewPhoto = "default_user.png";
        [ObservableProperty] private List<string> selectedWarehouseIds = new();

        public List<UserRole> AvailableRoles => Enum.GetValues(typeof(UserRole)).Cast<UserRole>().ToList();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSelectWarehouses))] 
        private UserRole selectedRole = UserRole.manager;
        public bool CanSelectWarehouses => SelectedRole != UserRole.admin;

        public UserCreateFromAdminViewModel(AuthService authService, UserContextService userContext)
        {
            _authService = authService;
            _userContext = userContext;
        }

        [RelayCommand]
        private async Task SelectPhoto()
        {
            var result = await MediaPicker.PickPhotoAsync();
            if (result != null)
            {
                Photo = result;
                var stream = await result.OpenReadAsync();
                PreviewPhoto = ImageSource.FromStream(() => stream);
            }
        }

        [RelayCommand]
        private async Task OpenWarehousePicker()
        {
            var parameters = new Dictionary<string, object>
        {
            { "InitialSelectedIds", SelectedWarehouseIds }
        };
            await ShellService.NavigateTo(nameof(WarehousePickerPage), parameters);
        }

        [RelayCommand]
        private async Task Save()
        {
            if(string.IsNullOrEmpty(Name) ||
                string.IsNullOrEmpty(Email) ||
                string.IsNullOrEmpty(Password)
                )
            {
                await Shell.Current.DisplayAlertAsync("Помилка", "Заповніть всі поля", "ОK");
                return;
            }

            await RunBusyTask(async () =>
            {
                var registerData = new UserRegister
                {
                    CompanyId = _userContext.CurrentUser.CompanyId,
                    Name = Name,
                    Email = Email,
                    Password = Password,
                    UserRole = SelectedRole,
                    WarehouseIds = SelectedRole == UserRole.admin ? null : SelectedWarehouseIds
                };

                await _authService.Register(registerData, Photo);
                await Shell.Current.DisplayAlertAsync("Успіх", "Користувача створено", "OK");
                await ShellService.GoBack();
            });
        }

        [RelayCommand]
        private async Task Cancel() => await ShellService.GoBack();
    }
}
