using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.AddressClass;

namespace InventorySystem_MAUI.ViewModel
{
    public partial class AddressViewModel : ObservableObject
    {
        private readonly AddressService _addressService;

        [ObservableProperty] private string country;
        [ObservableProperty] private string state;
        [ObservableProperty] private string city;
        [ObservableProperty] private string street;
        [ObservableProperty] private string houseNumber;
        [ObservableProperty] private double? latitude;
        [ObservableProperty] private double? longitude;
        [ObservableProperty] private bool isBusy;

        public AddressViewModel(AddressService addressService)
        {
            _addressService = addressService;
        }

        [RelayCommand]
        private async Task GetByText()
        {
            IsBusy = true;
            var request = new Address { Country = Country, State = State, City = City, Street = Street, HouseNumber = HouseNumber };
            var result = await _addressService.GetByAddress(request, "address"); 
            UpdateFields(result);
            IsBusy = false;
        }

        [RelayCommand]
        private async Task GetByCoords()
        {
            if (Latitude == null || Longitude == null) return;
            IsBusy = true;
            var request = new Address(Latitude.Value, Longitude.Value);
            var result = await _addressService.GetByAddress(request, "location");
            UpdateFields(result);
            IsBusy = false;
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

            await ShellService.GoBack(new Dictionary<string, object> { { "SelectedAddress", finalAddress } } );
        }
    }
}
