using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
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
        [ObservableProperty] private UserRole selectedRole = UserRole.manager;
        [ObservableProperty] private FileResult? photo;
        [ObservableProperty] private List<string> selectedWarehouseIds = new();

        public List<UserRole> AvailableRoles => Enum.GetValues(typeof(UserRole)).Cast<UserRole>().ToList();

        public bool CanSelectWarehouses => SelectedRole != UserRole.admin;

        public UserCreateFromAdminViewModel(AuthService authService, UserContextService userContext)
        {
            _authService = authService;
            _userContext = userContext;
        }

        [RelayCommand]
        private async Task SelectPhoto()
        {
            Photo = await MediaPicker.PickPhotoAsync();
        }

        [RelayCommand]
        private async Task OpenWarehousePicker()
        {
            var parameters = new Dictionary<string, object>
        {
            { "InitialSelectedIds", SelectedWarehouseIds }
        };
            await ShellService.NavigateTo(nameof(View.WarehousePickerPage), parameters);
        }

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlertAsync("Помилка", "Заповніть усі обов'язкові поля", "OK");
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
