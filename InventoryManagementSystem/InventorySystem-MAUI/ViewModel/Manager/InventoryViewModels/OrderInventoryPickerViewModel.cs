using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_API.Inventory.Models;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Inventory;
using InventorySystem_Shared.Inventory.Manufacturer;
using System.Collections.ObjectModel;

namespace InventorySystem_MAUI.ViewModel;

[QueryProperty(nameof(WarehouseId), "WarehouseId")]
[QueryProperty(nameof(ExistingSelection), "ExistingSelection")]
public partial class OrderInventoryPickerViewModel : BaseViewModel
{
    private readonly IInventoryService _inventoryService;
    private readonly IManufacturerService _manufacturerService;
    private CancellationTokenSource? _debounceTokenSource;

    [ObservableProperty] private string warehouseId = string.Empty;
    [ObservableProperty] private Dictionary<string, int> existingSelection = new();
    [ObservableProperty] private ObservableCollection<SelectableInventoryItem> items = new();
    [ObservableProperty] private ObservableCollection<InventoryManufacturer> manufacturers = new();
    [ObservableProperty] private string searchText;
    [ObservableProperty] private InventoryQuery query = new() { Page = 1, PageSize = 10 };
    [ObservableProperty] private bool isFilterVisible;
    [ObservableProperty] private bool canGoNext;
    [ObservableProperty] private int currentPage = 1;

    private Dictionary<string, int> _selectedItems = new();

    public List<InventoryType?> InventoryTypes { get; } = Enum.GetValues(typeof(InventoryType))
        .Cast<InventoryType?>()
        .Prepend(null)
        .ToList();

    public OrderInventoryPickerViewModel(IInventoryService inventoryService, IManufacturerService manufacturerService)
    {
        _inventoryService = inventoryService;
        _manufacturerService = manufacturerService;
    }

    async partial void OnWarehouseIdChanged(string value) => await LoadData();

    partial void OnExistingSelectionChanged(Dictionary<string, int> value)
    {
        if (value != null) _selectedItems = new Dictionary<string, int>(value);
    }

    async partial void OnSearchTextChanged(string value)
    {
        _debounceTokenSource?.Cancel();
        _debounceTokenSource = new CancellationTokenSource();
        var token = _debounceTokenSource.Token;
        try
        {
            await Task.Delay(500, token);
            if (!token.IsCancellationRequested)
            {
                Query.Name = value;
                CurrentPage = 1;
                await LoadData();
            }
        }
        catch (TaskCanceledException) { }
    }

    [RelayCommand]
    public async Task LoadData()
    {
        await RunBusyTask(async () =>
        {
            Query.Page = CurrentPage;
            var result = await _inventoryService.GetItemsByWarehouse(WarehouseId, Query);

            var wrappedItems = result.Select(x =>
            {
                bool isSelected = _selectedItems.ContainsKey(x.Id);
                var selectable = new SelectableInventoryItem(this)
                {
                    Item = x,
                    IsSelected = isSelected,
                    SelectedQuantity = isSelected ? _selectedItems[x.Id] : x.Quantity
                };
                return selectable;
            });

            Items = new ObservableCollection<SelectableInventoryItem>(wrappedItems);
            CanGoNext = result.Count == Query.PageSize;
            IsFilterVisible = false;
        });
    }

    public void SyncItem(SelectableInventoryItem selectable)
    {
        if (selectable.IsSelected)
            _selectedItems[selectable.Item.Id] = selectable.SelectedQuantity;
        else
            _selectedItems.Remove(selectable.Item.Id);
    }

    [RelayCommand]
    private void ToggleSelection(SelectableInventoryItem selectable)
    {
        selectable.IsSelected = !selectable.IsSelected;
        if (selectable.IsSelected && !_selectedItems.ContainsKey(selectable.Item.Id))
            selectable.SelectedQuantity = selectable.Item.Quantity;

        SyncItem(selectable);
    }

    [RelayCommand]
    private void SelectAllOnPage()
    {
        foreach (var item in Items)
        {
            item.SelectedQuantity = item.Item.Quantity;
            item.IsSelected = true;
            SyncItem(item);
        }
    }

    [RelayCommand]
    private async Task SelectAllInWarehouse()
    {
        await RunBusyTask(async () =>
        {
            var allItemsQuery = new InventoryQuery
            {
                Name = Query.Name,
                Description = Query.Description,
                MinPrice = Query.MinPrice,
                MaxPrice = Query.MaxPrice,
                InventoryType = Query.InventoryType,
                Manufacturer = Query.Manufacturer,
                Page = 1,
                PageSize = 9999
            };

            var allItems = await _inventoryService.GetItemsByWarehouse(WarehouseId, allItemsQuery);

            foreach (var inv in allItems)
            {
                _selectedItems[inv.Id] = inv.Quantity;
            }

            foreach (var item in Items)
            {
                item.SelectedQuantity = item.Item.Quantity;
                item.IsSelected = true;
            }

            await ShellService.DisplayAlert("Успіх", $"Вибрано всі товари за фільтром ({allItems.Count})", "OK");
        });
    }

    [RelayCommand]
    private async Task SaveAndClose()
    {
        var parameters = new Dictionary<string, object>
        {
            { "SelectedItemsDict", _selectedItems }
        };
        await ShellService.GoBack(parameters);
    }

    [RelayCommand]
    private async Task ToggleFilter()
    {
        IsFilterVisible = !IsFilterVisible;
        if (IsFilterVisible && Manufacturers.Count == 0) await LoadManufacturers();
    }

    private async Task LoadManufacturers()
    {
        var list = await _manufacturerService.GetManufacturersAsync();
        Manufacturers = new ObservableCollection<InventoryManufacturer>(list);
    }

    [RelayCommand] private async Task NextPage() { CurrentPage++; await LoadData(); }
    [RelayCommand] private async Task PreviousPage() { if (CurrentPage > 1) { CurrentPage--; await LoadData(); } }
    [RelayCommand] private async Task ResetFilters() { Query = new InventoryQuery { Page = 1, PageSize = 10 }; CurrentPage = 1; await LoadData(); }
    [RelayCommand] private async Task Cancel() => await ShellService.GoBack();
}

public partial class SelectableInventoryItem : ObservableObject
{
    private readonly OrderInventoryPickerViewModel _parent;
    public InventoryResponse Item { get; set; }

    public SelectableInventoryItem(OrderInventoryPickerViewModel parent) => _parent = parent;

    [ObservableProperty] private bool isSelected;

    private int _selectedQuantity = 1;
    public int SelectedQuantity
    {
        get => _selectedQuantity;
        set
        {
            var validValue = value;
            if (validValue > Item.Quantity) validValue = Item.Quantity;
            if (validValue < 1) validValue = 1;

            if (SetProperty(ref _selectedQuantity, validValue))
            {
                _parent.SyncItem(this);
            }
        }
    }
}