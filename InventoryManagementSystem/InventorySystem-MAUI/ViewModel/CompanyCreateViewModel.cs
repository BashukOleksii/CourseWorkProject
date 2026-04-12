using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.View;
using InventorySystem_Shared.AddressClass;
using InventorySystem_Shared.Company;


namespace InventorySystem_MAUI.ViewModel
{
    [QueryProperty(nameof(SelectedAddress), "SelectedAddress")]
    public partial class CompanyCreateViewModel : BaseViewModel
    {
        private readonly CompanyService _companyService;

        [ObservableProperty] private string name;
        [ObservableProperty] private string description;
        [ObservableProperty] private string phone;
        [ObservableProperty] private Address? selectedAddress;
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
            await ShellService.NavigateTo(nameof(AddressCreatePage), new Dictionary<string, object>{ { "InitialAddress", selectedAddress } });
        }

        [RelayCommand]
        private async Task SaveCompany()
        {
            if (string.IsNullOrEmpty(Name) ||
                   SelectedAddress == null ||
                   string.IsNullOrEmpty(Description) ||
                   string.IsNullOrEmpty(Phone))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Заповніть всі поля", "OK");
                return;
            }

            await RunBusyTask(async () =>
            {
                var dto = new CompanyDTO
                {
                    Name = Name,
                    Description = Description,
                    Phone = Phone,
                    Address = SelectedAddress
                };

                var id = await _companyService.CreateCompany(dto);
                await Shell.Current.DisplayAlertAsync("Success", "Успіх", "OK");
                await ShellService.NavigateTo(nameof(UserCreatePage), new Dictionary<string, object>
                    {
                        { "CompanyId", id }
                    });
            });

           
        }
    }
}
