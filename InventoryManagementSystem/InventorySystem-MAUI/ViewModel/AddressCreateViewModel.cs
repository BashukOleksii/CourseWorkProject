using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.AddressClass;

namespace InventorySystem_MAUI.ViewModel
{
    public partial class AddressCreateViewModel : BaseViewModel
    {
        private readonly AddressService _addressService;

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

        [RelayCommand]
        private async Task GetByText() {
            await RunBusyTask(async () =>
            {
                var request = new Address(country, state, district, city, street, houseNumber);
                var result = await _addressService.GetByAddress(request, "address");
                UpdateFields(result);
                await Shell.Current.DisplayAlertAsync("Адреса", "Адресу знайдено за вказаними полями", "OK");
            });
       }

        

        [RelayCommand]
        private async Task GetByCoords()
        {
            await RunBusyTask(async () =>
            {
                if (Latitude == null || Longitude == null) return;
                var request = new Address(Latitude.Value, Longitude.Value);
                var result = await _addressService.GetByAddress(request, "location");
                UpdateFields(result);

                await Shell.Current.DisplayAlertAsync("Адреса", "Адресу знайдено за вказаними координатами", "OK");
            });
        }

        private void UpdateFields(Address addr)
        {
            if (addr == null) return;
            Country = addr.Country;
            State = addr.State;
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
                    Country = Country,
                    State = State,
                    City = City,
                    Street = Street,
                    HouseNumber = HouseNumber,
                    Latitude = Latitude,
                    Longitude = Longitude
                };
                finalAddress.Normalize();

                await ShellService.GoBack(new Dictionary<string, object> { { "SelectedAddress", finalAddress } });
            });

        }
    }
}
