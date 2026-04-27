using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.AddressClass;
using InventorySystem_Shared.Company;

namespace InventorySystem_MAUI.ViewModel;

[QueryProperty(nameof(SelectedAddress), "SelectedAddress")]
public partial class CompanyViewModel : BaseViewModel
{
    private readonly CompanyService _companyService;

    [ObservableProperty] private CompanyResponse company;
    [ObservableProperty] private string name;
    [ObservableProperty] private string description;
    [ObservableProperty] private string phone;
    [ObservableProperty] private Address address;

    [ObservableProperty] private Address selectedAddress;

    public CompanyViewModel(CompanyService companyService)
    {
        _companyService = companyService;
        Task.Run(async () => await LoadCompany());
    }

    private async Task LoadCompany()
    {
        await RunBusyTask(async () =>
        {
            Company = await _companyService.GetMyCompany();
            Name = Company.Name;
            Description = Company.Description;
            Phone = Company.Phone;
            Address = Company.Address;
        });
    }

    [RelayCommand]
    private async Task EditAddress()
    {
        var parameters = new Dictionary<string, object>
        {
            { "InitialAddress", Address }
        };
        await ShellService.NavigateTo(nameof(View.AddressCreatePage), parameters);
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        await RunBusyTask(async () =>
        {
            var update = new CompanyUpdate
            {
                Name = Name,
                Description = Description,
                Phone = Phone,
                Address = Address
            };

            await _companyService.UpdateMyCompany(update);
        });
    }
}