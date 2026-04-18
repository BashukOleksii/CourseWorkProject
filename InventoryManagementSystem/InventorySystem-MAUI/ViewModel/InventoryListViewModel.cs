using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.View;
using InventorySystem_Shared.Inventory;
using System.Collections.ObjectModel;

namespace InventorySystem_MAUI.ViewModel;

[QueryProperty(nameof(WarehouseId), "WarehouseId")]
[QueryProperty(nameof(WarehouseName), "WarehouseName")]
public partial class InventoryListViewModel : BaseViewModel
{
    private readonly InventoryService _inventoryService;
    private CancellationTokenSource? _searchCts;

    [ObservableProperty] private string warehouseId = string.Empty;
    [ObservableProperty] private string warehouseName = string.Empty; 
    [ObservableProperty] private ObservableCollection<InventoryResponse> items = new();

    [ObservableProperty] private string itemSearchText = string.Empty; 

    [ObservableProperty] private int currentPage = 1;
    [ObservableProperty] private bool canGoNext;

    public InventoryListViewModel(InventoryService inventoryService) => _inventoryService = inventoryService;

    async partial void OnWarehouseIdChanged(string value) => await LoadItems();

    async partial void OnItemSearchTextChanged(string value)
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(500, _searchCts.Token);
            CurrentPage = 1;
            await LoadItems();
        }
        catch (TaskCanceledException) { }
    }

    [RelayCommand]
    public async Task LoadItems()
    {
        if (string.IsNullOrEmpty(WarehouseId)) return;

        await RunBusyTask(async () =>
        {
            var query = new InventoryQuery
            {
                Name = ItemSearchText, 
                Page = CurrentPage,
                PageSize = 10
            };
            var result = await _inventoryService.GetItemsByWarehouse(WarehouseId, query);

            Items = new ObservableCollection<InventoryResponse>(result);
            CanGoNext = result.Count == 10;
        });
    }

    [RelayCommand]
    private async Task AddItem() =>
        await ShellService.NavigateTo(nameof(InventoryDetailsPage), new Dictionary<string, object> { { "WarehouseId", WarehouseId } });

    [RelayCommand]
    private async Task EditItem(InventoryResponse item) =>
        await ShellService.NavigateTo(nameof(InventoryDetailsPage), new Dictionary<string, object> { { "Id", item.Id }, { "WarehouseId", WarehouseId } });
    [RelayCommand]
    private async Task DeleteItem(InventoryResponse item)
    {
        bool confirm = await Shell.Current.DisplayAlertAsync("Видалення", $"Видалити {item.Name}?", "Так", "Ні");
        if (confirm)
        {
            await RunBusyTask(async () => {
                await _inventoryService.DeleteById(item.Id);
                Items.Remove(item);
            });
        }
    }

    [RelayCommand]
    private async Task GoToAggregation()
    {
        await ShellService.NavigateTo(nameof(InventoryAggregationPage), new Dictionary<string, object>{ { "WarehouseId", WarehouseId } });
    }

    [RelayCommand] private async Task NextPage() { CurrentPage++; await LoadItems(); }
    [RelayCommand] private async Task PreviousPage() { if (CurrentPage > 1) { CurrentPage--; await LoadItems(); } }
}