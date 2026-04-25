using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.AddressClass;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventorySystem_MAUI.ViewModel
{
    [QueryProperty(nameof(InitialAddress), "InitialAddress")]
    public partial class AddressCreateViewModel : BaseViewModel
    {
        private readonly AddressService _addressService;

        [ObservableProperty] private Address? initialAddress;

        [ObservableProperty] private string? country;
        [ObservableProperty] private string? state;
        [ObservableProperty] private string? district;
        [ObservableProperty] private string? city;
        [ObservableProperty] private string? street;
        [ObservableProperty] private string? houseNumber;
        [ObservableProperty] private double? latitude;
        [ObservableProperty] private double? longitude;

        public AddressCreateViewModel(AddressService addressService)
        {
            _addressService = addressService;
        }

        async partial void OnInitialAddressChanged(Address? value)
        {
            if (value == null) return;
            UpdateFields(value);
        }

        [RelayCommand]
        private async Task GetByText()
        {
            await RunBusyTask(async () =>
            {
                var request = new Address
                {
                    Country = Country,
                    State = State,
                    District = District,
                    City = City,
                    Street = Street,
                    HouseNumber = HouseNumber
                };

                var result = await _addressService.GetByAddress(request, "address");
                if (result != null)
                {
                    UpdateFields(result);
                    await Shell.Current.DisplayAlertAsync("Успіх", "Адресу уточнено через сервіс", "OK");
                }
            });
        }

        [RelayCommand]
        private async Task GetByCoords()
        {
            if (Latitude == null || Longitude == null)
            {
                await Shell.Current.DisplayAlertAsync("Помилка", "Введіть широту та довготу", "OK");
                return;
            }

            await RunBusyTask(async () =>
            {
                var request = new Address(Latitude.Value, Longitude.Value);
                var result = await _addressService.GetByAddress(request, "location");

                if (result != null)
                {
                    UpdateFields(result);
                    await Shell.Current.DisplayAlertAsync("Успіх", "Адресу знайдено за координатами", "OK");
                }
            });
        }

        private void UpdateFields(Address addr)
        {
            if (addr == null) return;

            Country = addr.Country;
            State = addr.State;
            District = addr.District;
            City = addr.City;
            Street = addr.Street;
            HouseNumber = addr.HouseNumber;
            Latitude = addr.Latitude;
            Longitude = addr.Longitude;
        }

        [RelayCommand]
        private async Task SaveAndClose()
        {
            await RunBusyTask(async () =>
            {
                var finalAddress = new Address
                {
                    Country = Country ?? string.Empty,
                    State = State ?? string.Empty,
                    District = District ?? string.Empty,
                    City = City ?? string.Empty,
                    Street = Street ?? string.Empty,
                    HouseNumber = HouseNumber ?? string.Empty,
                    Latitude = Latitude ?? 0,
                    Longitude = Longitude ?? 0
                };

                finalAddress.Normalize();

                var navigationParameter = new Dictionary<string, object>
                {
                    { "SelectedAddress", finalAddress }
                };

                await ShellService.GoBack(navigationParameter);
            });
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await ShellService.GoBack();
        }
    }
}