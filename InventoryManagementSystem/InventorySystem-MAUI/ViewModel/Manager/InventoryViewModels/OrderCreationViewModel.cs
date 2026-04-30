using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.View.Manager.InventoryPages;
using InventorySystem_Shared.AddressClass;
using InventorySystem_Shared.Company;

namespace InventorySystem_MAUI.ViewModel;

[QueryProperty(nameof(SelectedAddress), "SelectedAddress")]
[QueryProperty(nameof(SelectedInventoryIds), "SelectedInventoryIds")]
[QueryProperty(nameof(WarehouseId), "WarehouseId")]
public partial class OrderCreationViewModel : BaseViewModel
{
    private readonly IInventoryService _inventoryService;

    [ObservableProperty] private string warehouseId;

    [ObservableProperty] private string customerName;
    [ObservableProperty] private string customerPhone;
    [ObservableProperty] private Address selectedAddress;

    [ObservableProperty] private List<string> selectedInventoryIds = new();

    [ObservableProperty] private string addressSummary = "Адресу не обрано";
    [ObservableProperty] private string itemsSummary = "Товари не обрано";

    public OrderCreationViewModel(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    partial void OnSelectedAddressChanged(Address value)
    {
        if (value != null)
            AddressSummary = $"{value.City}, {value.Street} {value.HouseNumber}";
    }

    partial void OnSelectedInventoryIdsChanged(List<string> value)
    {
        if (value != null && value.Any())
            ItemsSummary = $"Обрано товарів: {value.Count}";
        else
            ItemsSummary = "Товари не обрано";
    }

    [RelayCommand]
    private async Task OpenAddressPicker()
    {
        await ShellService.NavigateTo(nameof(View.AddressCreatePage), new Dictionary<string, object>
        {
            { "InitialAddress", SelectedAddress }
        });
    }

    [RelayCommand]
    private async Task OpenInventoryPicker()
    {
       await ShellService.NavigateTo(nameof(OrderInventoryPickerPage), new Dictionary<string, object>
        {
            { "WarehouseId", WarehouseId },
            { "ExistingSelection", SelectedInventoryIds }
        });
    }

    [RelayCommand]
    private async Task CreateOrder()
    {
        if (string.IsNullOrWhiteSpace(CustomerName) || string.IsNullOrWhiteSpace(CustomerPhone) || SelectedAddress == null)
        {
            await ShellService.DisplayAlert("Помилка", "Заповніть дані замовника та адресу", "OK");
            return;
        }

        if (SelectedInventoryIds == null || !SelectedInventoryIds.Any())
        {
            await ShellService.DisplayAlert("Помилка", "Виберіть хоча б один товар", "OK");
            return;
        }

        await RunBusyTask(async () =>
        {
            var provider = new CompanyDTO
            {
                Name = CustomerName,
                Phone = CustomerPhone,
                Address = SelectedAddress,
                Description = "Замовлення через додаток"
            };

            var pdfBytes = await _inventoryService.GetSalesReport(WarehouseId, SelectedInventoryIds.ToArray(), provider);

            var fileName = $"Invoice_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            var path = Path.Combine(FileSystem.CacheDirectory, fileName);
            await File.WriteAllBytesAsync(path, pdfBytes);
            await Launcher.Default.OpenAsync(new OpenFileRequest("Накладна", new ReadOnlyFile(path)));
            
            await ShellService.GoBack();
        });
    }

    [RelayCommand]
    private async Task Cancel() => await ShellService.GoBack();
}