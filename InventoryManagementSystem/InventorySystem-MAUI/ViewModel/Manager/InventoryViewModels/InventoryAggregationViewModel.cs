using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_API.Inventory.Models;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Inventory;
using System.Collections.ObjectModel;

namespace InventorySystem_MAUI.ViewModel;

[QueryProperty(nameof(WarehouseId), "WarehouseId")]
public partial class InventoryAggregationViewModel : BaseViewModel
{
    private readonly IInventoryService _inventoryService;

    [ObservableProperty] private string warehouseId = string.Empty;
    [ObservableProperty] private ObservableCollection<InventoryResponse> items = new();

    [ObservableProperty] private InventoryQuery query = new() { Page = 1, PageSize = 10, SortDescending = true };

    [ObservableProperty] private bool isFilterVisible;
    [ObservableProperty] private bool canGoNext;

    public List<InventoryType?> InventoryTypes { get; } = Enum.GetValues(typeof(InventoryType))
        .Cast<InventoryType?>()
        .Prepend(null)
        .ToList();

    public InventoryAggregationViewModel(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [RelayCommand]
    private void ToggleFilter() => IsFilterVisible = !IsFilterVisible;

    [RelayCommand]
    public async Task LoadData()
    {
        await RunBusyTask(async () =>
        {
            var result = await _inventoryService.GetItemsByWarehouse(WarehouseId, Query);
            Items = new ObservableCollection<InventoryResponse>(result);
            CanGoNext = result.Count == Query.PageSize;
            IsFilterVisible = false;
        });
    }

    [RelayCommand]
    private async Task ResetFilters()
    {
        Query = new InventoryQuery { Page = 1, PageSize = 10 };
        await LoadData();
    }

    [RelayCommand]
    private async Task GenerateReport()
    {
        await RunBusyTask(async () =>
        {
            var bytes = await _inventoryService.GetInventoryReport(WarehouseId, Query);
            await SaveAndOpenFile(bytes, $"Report_{DateTime.Now:yyyyMMdd}.pdf");
        });
    }

    [RelayCommand]
    private async Task ExportData(string format) 
    {
        await RunBusyTask(async () =>
        {
            var bytes = await _inventoryService.ExportData(WarehouseId, format);
            await SaveAndOpenFile(bytes, $"Inventory_Export.{format}");
        });
    }

    [RelayCommand]
    private async Task ImportData()
    {
        var result = await FilePicker.Default.PickAsync();
        if (result != null)
        {
            await RunBusyTask(async () =>
            {
                await _inventoryService.ImportData(WarehouseId, result);
                await LoadData();
            });
        }
    }

    private async Task SaveAndOpenFile(byte[] data, string fileName)
    {
        var path = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllBytesAsync(path, data);
        await Launcher.Default.OpenAsync(new OpenFileRequest("Перегляд файлу", new ReadOnlyFile(path)));
    }


    [RelayCommand]
    private async Task OpenDetails(InventoryResponse item) =>
        await Shell.Current.GoToAsync(nameof(InventoryDetailsViewModel), new Dictionary<string, object>
        {
            { "Id", item.Id }
        });

    [RelayCommand]
    private async Task DeleteItem(InventoryResponse item)
    {
        bool confirm = await Shell.Current.DisplayAlertAsync("Видалення", $"Видалити {item.Name}?", "Так", "Ні");
        if (confirm)
        {
            await RunBusyTask(async () =>
            {
                await _inventoryService.DeleteById(item.Id);
                Items.Remove(item);
            });
        }
    }


    [RelayCommand]
    private async Task NextPage() { Query.Page++; await LoadData(); }

    [RelayCommand]
    private async Task PreviousPage() { if (Query.Page > 1) { Query.Page--; await LoadData(); } }
}