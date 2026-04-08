using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.View;
using InventorySystem_Shared.AddressClass;
using InventorySystem_Shared.Company;


namespace InventorySystem_MAUI.ViewModel
{
    [QueryProperty(nameof(Address), "SelectedAddress")]
    public partial class CompanyCreateViewModel : BaseViewModel
    {
        private readonly CompanyService _companyService;

        [ObservableProperty] private string name;
        [ObservableProperty] private string description;
        [ObservableProperty] private string phone;
        [ObservableProperty] private Address selectedAddress;
        [ObservableProperty] private string addressDisplay = "Адресу не обрано";

        public CompanyCreateViewModel(CompanyService companyService)
        {
            _companyService = companyService;
        }

        partial void OnSelectedAddressChanged(Address value)
        {
            if (value != null)
                AddressDisplay = $"{value.City}, {value.Street} {value.HouseNumber}";
        }

        [RelayCommand]
        private async Task OpenAddressPicker()
        {
            await ShellService.NavigateTo(nameof(AddressCreatePage));
        }

        [RelayCommand]
        private async Task SaveCompany()
        {
            await RunBusyTask(async () =>
            {
                if (string.IsNullOrEmpty(Name) ||
                    SelectedAddress == null ||
                    string.IsNullOrEmpty(Description) ||
                    string.IsNullOrEmpty(Phone))
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Заповніть всі поля", "OK");
                    return;
                }

                var dto = new CompanyDTO
                {
                    Name = Name,
                    Description = Description,
                    Phone = Phone,
                    Address = SelectedAddress
                };

                // Додати перехід на створення користувача із id
                var id = await _companyService.CreateCompany(dto);
                await Shell.Current.DisplayAlertAsync("Success", "Успіх", "OK");
            });
        }
    }
}
