using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_API.Inventory.Models;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Inventory;
using System.Collections.ObjectModel;

namespace InventorySystem_MAUI.ViewModel;

[QueryProperty(nameof(WarehouseId), "WarehouseId")]
[QueryProperty(nameof(ExistingSelection), "ExistingSelection")]
public partial class OrderInventoryPickerViewModel : BaseViewModel
{
    private readonly IInventoryService _inventoryService;
    private CancellationTokenSource? _debounceTokenSource;

    [ObservableProperty] private string warehouseId = string.Empty;
    [ObservableProperty] private List<string> existingSelection = new();
    [ObservableProperty] private ObservableCollection<SelectableInventoryItem> items = new();

    [ObservableProperty] private InventoryQuery query = new() { Page = 1, PageSize = 10 };

    [ObservableProperty] private bool isFilterVisible;
    [ObservableProperty] private bool canGoNext;
    [ObservableProperty] private int currentPage = 1;

    private HashSet<string> _selectedIds = new();

    public List<InventoryType?> InventoryTypes { get; } = Enum.GetValues(typeof(InventoryType))
        .Cast<InventoryType?>()
        .Prepend(null)
        .ToList();

    public OrderInventoryPickerViewModel(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    async partial void OnWarehouseIdChanged(string value) => await LoadData();

    partial void OnExistingSelectionChanged(List<string> value)
    {
        if (value != null)
            _selectedIds = new HashSet<string>(value);
    }

    async partial void OnQueryChanged(InventoryQuery value) => await LoadDataDebounced();

    [RelayCommand]
    public async Task LoadData()
    {
        _debounceTokenSource?.Cancel();

        await RunBusyTask(async () =>
        {
            Query.Page = CurrentPage;
            var result = await _inventoryService.GetItemsByWarehouse(WarehouseId, Query);

            var wrappedItems = result.Select(x => new SelectableInventoryItem
            {
                Item = x,
                IsSelected = _selectedIds.Contains(x.Id)
            });

            Items = new ObservableCollection<SelectableInventoryItem>(wrappedItems);
            CanGoNext = result.Count == Query.PageSize;
        });
    }

    private async Task LoadDataDebounced()
    {
        _debounceTokenSource?.Cancel();
        _debounceTokenSource = new CancellationTokenSource();
        var token = _debounceTokenSource.Token;

        try
        {
            await Task.Delay(500, token); 
            if (!token.IsCancellationRequested)
            {
                CurrentPage = 1;
                await LoadData();
            }
        }
        catch (TaskCanceledException) { }
    }

    [RelayCommand]
    private void ToggleSelection(SelectableInventoryItem selectable)
    {
        selectable.IsSelected = !selectable.IsSelected;
        if (selectable.IsSelected) _selectedIds.Add(selectable.Item.Id);
        else _selectedIds.Remove(selectable.Item.Id);
    }

    [RelayCommand]
    private async Task SelectAllOnPage()
    {
        foreach (var item in Items)
        {
            item.IsSelected = true;
            _selectedIds.Add(item.Item.Id);
        }
    }

    [RelayCommand]
    private async Task SelectAllInWarehouse()
    {
        await RunBusyTask(async () =>
        {
            var allItemsQuery = new InventoryQuery { Page = 1, PageSize = null };
            var allItems = await _inventoryService.GetItemsByWarehouse(WarehouseId, allItemsQuery);

            foreach (var id in allItems.Select(x => x.Id))
                _selectedIds.Add(id);

            foreach (var item in Items)
                item.IsSelected = true;

            await ShellService.DisplayAlert("Успіх", $"Вибрано всі товари ({allItems.Count})", "OK");
        });
    }

    [RelayCommand]
    private async Task SaveAndClose()
    {
        var parameters = new Dictionary<string, object>
        {
            { "SelectedInventoryIds", _selectedIds.ToList() }
        };
        await ShellService.GoBack(parameters);
    }

    [RelayCommand] private void ToggleFilter() => IsFilterVisible = !IsFilterVisible;

    [RelayCommand]
    private async Task NextPage() { CurrentPage++; await LoadData(); }

    [RelayCommand]
    private async Task PreviousPage() { if (CurrentPage > 1) { CurrentPage--; await LoadData(); } }

    [RelayCommand]
    private async Task ResetFilters()
    {
        Query = new InventoryQuery { Page = 1, PageSize = 10 };
        CurrentPage = 1;
        await LoadData();
    }

    [RelayCommand] private async Task Cancel() => await ShellService.GoBack();
}

public partial class SelectableInventoryItem : ObservableObject
{
    public InventoryResponse Item { get; set; }
    [ObservableProperty] private bool isSelected;
}