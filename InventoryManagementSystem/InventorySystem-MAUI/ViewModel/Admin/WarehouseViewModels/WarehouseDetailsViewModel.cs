using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.View;
using InventorySystem_Shared.AddressClass;
using InventorySystem_Shared.Warehouse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventorySystem_MAUI.ViewModel
{
    [QueryProperty(nameof(WarehouseId), "Id")]
    [QueryProperty(nameof(SelectedAddress), "SelectedAddress")]
    public partial class WarehouseDetailsViewModel : BaseViewModel
    {
        private readonly WarehouseService _warehouseService;

        [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasId))] private string warehouseId;
        [ObservableProperty] private string title = "Новий склад";
        [ObservableProperty] private string addressDisplay = "Адресу не обрано";

        [ObservableProperty] private string name;
        [ObservableProperty] private string description;
        [ObservableProperty] private double area;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasAddress))]
        private Address? selectedAddress;

        public bool HasAddress => SelectedAddress != null;
        public bool HasId => !string.IsNullOrEmpty(warehouseId);

        public WarehouseDetailsViewModel(WarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        partial void OnSelectedAddressChanged(Address value)
        {
            if (value != null)
                AddressDisplay = $"{value.City}, {value.Street} {value.HouseNumber}";
        }

        async partial void OnWarehouseIdChanged(string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            Title = "Редагування складу";
            await RunBusyTask(async () =>
            {
                var warehouse = await _warehouseService.GetWarehouseById(value);
                if (warehouse != null)
                {
                    Name = warehouse.Name;
                    Description = warehouse.Description;
                    Area = warehouse.Area;
                    SelectedAddress = warehouse.Address;
                }
            });
        }

        [RelayCommand]
        private async Task OpenAddressPicker()
        {
            var parameters = new Dictionary<string, object>();

            if (SelectedAddress != null)
                parameters.Add("InitialAddress", SelectedAddress);
            

            await ShellService.NavigateTo(nameof(View.AddressCreatePage), parameters);
        }

        [RelayCommand]
        private async Task Save()
        {

            await RunBusyTask(async () =>
            {
                if (string.IsNullOrEmpty(WarehouseId))
                {
                    var dto = new WarehouseDTO
                    {
                        Name = Name,
                        Description = Description ?? "",
                        Area = Area,
                        Address = SelectedAddress
                    };
                    await _warehouseService.CreateWarehouse(dto);
                    await Shell.Current.DisplayAlertAsync("Succes", "Склад успішно створено", "OK");
                }
                else
                {
                    var update = new WarehouseUpdate
                    {
                        Name = Name,
                        Description = Description ?? "",
                        Area = Area,
                        Address = SelectedAddress
                    };
                    await _warehouseService.UpdateWarehouse(WarehouseId, update);
                    await Shell.Current.DisplayAlertAsync("Succes", "Дані оновлено", "OK");
                }

                await ShellService.AbsoluteOpenPage(nameof(WarehouseListPage));
            });
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await ShellService.GoBack();
        }
    }
}