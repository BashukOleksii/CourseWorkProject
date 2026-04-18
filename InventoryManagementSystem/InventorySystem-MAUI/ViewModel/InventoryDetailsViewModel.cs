using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Inventory;
using InventorySystem_Shared.Inventory.Manufacturer;
using InventorySystem_API.Inventory.Models;
using System.Collections.ObjectModel;

namespace InventorySystem_MAUI.ViewModel;

[QueryProperty(nameof(WarehouseId), "WarehouseId")]
[QueryProperty(nameof(ItemId), "Id")]
public partial class InventoryDetailsViewModel : BaseViewModel
{
    private readonly InventoryService _inventoryService;
    private readonly ManufacturerService _manufacturerService;

    [ObservableProperty] private string warehouseId = string.Empty;
    [ObservableProperty] private string itemId = string.Empty;
    [ObservableProperty] private string title = "Новий товар";

    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private double price;
    [ObservableProperty] private int quantity;
    [ObservableProperty] private InventoryType selectedType = InventoryType.Electronics;
    [ObservableProperty] private InventoryManufacturer? selectedManufacturer;

    [ObservableProperty] private FileResult? photoFile;
    [ObservableProperty] private ImageSource? previewImage;

    [ObservableProperty] private ObservableCollection<CustomFieldItem> dynamicFields = new();

    [ObservableProperty] private ObservableCollection<InventoryManufacturer> manufacturers = new();
    public List<InventoryType> InventoryTypes => Enum.GetValues(typeof(InventoryType)).Cast<InventoryType>().ToList();

    public InventoryDetailsViewModel(InventoryService inventoryService, ManufacturerService manufacturerService)
    {
        _inventoryService = inventoryService;
        _manufacturerService = manufacturerService;
    }

    [RelayCommand]
    public async Task Initialize()
    {
        await RunBusyTask(async () =>
        {
            var localManufacturers = await _manufacturerService.GetManufacturersAsync();
            Manufacturers = new ObservableCollection<InventoryManufacturer>(localManufacturers);

            if (!string.IsNullOrEmpty(ItemId))
            {
                Title = "Редагування";
                var item = await _inventoryService.GetById(ItemId);

                Name = item.Name;
                Description = item.Description;
                Price = item.Price;
                Quantity = item.Quantity;
                SelectedType = item.InventoryType;

                SelectedManufacturer = Manufacturers.FirstOrDefault(m => m.Name == item.Manufacturer?.Name);

                if (!string.IsNullOrEmpty(item.PhotoURI))
                    PreviewImage = ImageSource.FromUri(new Uri(Conection.BaseURI + item.PhotoURI));

                if (item.CustomFileds != null)
                {
                    var fieldList = item.CustomFileds.Select(kvp => new CustomFieldItem { Key = kvp.Key, Value = kvp.Value });
                    DynamicFields = new ObservableCollection<CustomFieldItem>(fieldList);
                }
            }
        });
    }

    [RelayCommand]
    private async Task PickPhoto()
    {
        var result = await MediaPicker.PickPhotoAsync();
        if (result != null)
        {
            PhotoFile = result;
            var stream = await result.OpenReadAsync();
            PreviewImage = ImageSource.FromStream(() => stream);
        }
    }

    [RelayCommand]
    private void AddField() => DynamicFields.Add(new CustomFieldItem());

    [RelayCommand]
    private void RemoveField(CustomFieldItem field) => DynamicFields.Remove(field);

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name) || SelectedManufacturer == null)
        {
            await Shell.Current.DisplayAlertAsync("Помилка", "Заповніть назву та виберіть виробника", "OK");
            return;
        }

        await RunBusyTask(async () =>
        {
            var customFieldsDict = DynamicFields
                .Where(f => !string.IsNullOrWhiteSpace(f.Key))
                .ToDictionary(f => f.Key, f => f.Value ?? string.Empty);

            if (string.IsNullOrEmpty(ItemId))
            {
                var createDto = new InventoryCreate
                {
                    Name = Name,
                    Description = Description,
                    Price = Price,
                    Quantity = Quantity,
                    InventoryType = SelectedType,
                    Manufacturer = SelectedManufacturer,
                    CustomFileds = customFieldsDict
                };
                await _inventoryService.CreateItem(WarehouseId, createDto, PhotoFile);
            }
            else
            {
                var updateDto = new InventoryUpdate
                {
                    Name = Name,
                    Description = Description,
                    Price = Price,
                    Quantity = Quantity,
                    InventoryType = SelectedType,
                    Manufacturer = new InventoryManufacturerDTO { Name = SelectedManufacturer.Name, Country = SelectedManufacturer.Country },
                    CustomFileds = customFieldsDict
                };
                await _inventoryService.UpdateItem(ItemId, updateDto, PhotoFile);
            }

            await Shell.Current.GoToAsync("..");
        });
    }

    [RelayCommand]
    private async Task Cancel() => await ShellService.GoBack();
}

public partial class CustomFieldItem : ObservableObject
{
    [ObservableProperty] private string key = string.Empty;
    [ObservableProperty] private string value = string.Empty;
}