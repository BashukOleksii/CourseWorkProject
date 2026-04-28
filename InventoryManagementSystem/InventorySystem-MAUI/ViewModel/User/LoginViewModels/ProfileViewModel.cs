using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.View;
using InventorySystem_Shared.Company;
using InventorySystem_Shared.User;
using System.Buffers.Text;
using System.Runtime.CompilerServices;

namespace InventorySystem_MAUI.ViewModel
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;
        private readonly IUserContextService _userContext;

        [ObservableProperty] private string name;
        [ObservableProperty] private string email;
        [ObservableProperty] private UserRole userRole;
        [ObservableProperty] private CompanyResponse company;
        [ObservableProperty] private string photoUri;
        [ObservableProperty] private bool isEditMode;

        [ObservableProperty] private string editName;
        [ObservableProperty] private string editEmail;
        [ObservableProperty] private string newPassword;
        [ObservableProperty] private string confirmPassword;
        

        public ProfileViewModel(IUserService userService, ICompanyService companyService, IUserContextService userContext)
        {
            _userService = userService;
            _companyService = companyService;
            _userContext = userContext;
        }

        public async Task LoadData()
        {
            await RunBusyTask(async () =>
            {
                Name = _userContext.CurrentUser.Name;
                Email = _userContext.CurrentUser.Email;
                UserRole = _userContext.CurrentUser.UserRole;
                EditName = _userContext.CurrentUser.Name;
                EditEmail = _userContext.CurrentUser.Email;
                PhotoUri = Conection.BaseURI +  _userContext.CurrentUser.PhotoURI;
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
                await ShellService.DisplayAlert("Помилка", "Паролі не збігаються", "OK");
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

                var updatedUser = await _userService.UpdateUser(_userContext.CurrentUser.Id, update);

                Name = updatedUser.Name;
                Email = updatedUser.Email;
                PhotoUri = Conection.BaseURI + updatedUser.PhotoURI;

                IsEditMode = false;
                NewPassword = ConfirmPassword = string.Empty;

            });
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await ShellService.GoBack();
        }

        [RelayCommand]
        private async Task Logout()
        {
            bool confirm = await ShellService.DisplayAlert("Вихід", "Ви дійсно хочете вийти з акаунту?", "Так", "Ні");
            if (confirm)
            {
                _userContext.LogOut();
                await ShellService.AbsoluteOpenPage(nameof(WelcomePage));
            }
        }
    }
}
