using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Inventory.Manufacturer;
using System.Diagnostics.Metrics;

namespace InventorySystem_MAUI.ViewModel;

[QueryProperty(nameof(Manufacturer), "Manufacturer")]
public partial class ManufacturerDetailsViewModel : BaseViewModel
{
    private readonly IManufacturerService _service;
    private string? _originalName;

    [ObservableProperty] private InventoryManufacturer? manufacturer;
    [ObservableProperty] private string name;
    [ObservableProperty] private string country;
    [ObservableProperty] private string title = "Новий виробник";

    public ManufacturerDetailsViewModel(IManufacturerService service)
    {
        _service = service;
    }

    partial void OnManufacturerChanged(InventoryManufacturer? value)
    {
        if (value != null)
        {
            Name = value.Name;
            Country = value.Country;
            _originalName = value.Name;
            Title = "Редагування";
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Country))
        {
            await ShellService.DisplayAlert("Помилка", "Заповніть усі поля", "OK");
            return;
        }

        await RunBusyTask(async () =>
        {
            var newObj = new InventoryManufacturer { Name = Name, Country = Country };
            
            await _service.UpsertManufacturerAsync(newObj, _originalName);
            await ShellService.GoBack();
        });
    }

    [RelayCommand]
    private async Task Cancel() => await ShellService.GoBack();
}