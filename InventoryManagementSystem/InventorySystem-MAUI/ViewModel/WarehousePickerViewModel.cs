using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Warehouse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace InventorySystem_MAUI.ViewModel
{
    [QueryProperty(nameof(SelectedIds), "InitialSelectedIds")]
    public partial class WarehousePickerViewModel : BaseViewModel
    {
        private readonly WarehouseService _warehouseService;
        private CancellationTokenSource? _searchCts;

        [ObservableProperty] private ObservableCollection<WarehouseSelectableItem> warehouses = new();
        [ObservableProperty] private string searchText;
        [ObservableProperty] private int currentPage = 1;
        [ObservableProperty] private int pageSize = 6;
        [ObservableProperty] private bool canGoNext;

        [ObservableProperty] private List<string> selectedIds = new();
        private HashSet<string> _selectedIdsSet = new();

        public WarehousePickerViewModel(WarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        async partial void OnSearchTextChanged(string value)
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();

            try
            {
                await Task.Delay(500, _searchCts.Token);
                CurrentPage = 1; 
                await LoadWarehouses();
            }
            catch (TaskCanceledException) { }
        }

        async partial void OnSelectedIdsChanged(List<string> value)
        {
            if (value != null) _selectedIdsSet = new HashSet<string>(value);
            await LoadWarehouses();
        }

        [RelayCommand]
        public async Task LoadWarehouses()
        {
            await RunBusyTask(async () =>
            {
                var query = new WarehouseQuery
                {
                    Name = SearchText,
                    Page = CurrentPage,
                    PageSize = PageSize
                };

                var result = await _warehouseService.GetWarehouses(query);

                var selectableItems = result.Select(w => new WarehouseSelectableItem
                {
                    Warehouse = w,
                    IsSelected = _selectedIdsSet.Contains(w.Id)
                }).ToList();

                Warehouses = new ObservableCollection<WarehouseSelectableItem>(selectableItems);
                CanGoNext = result.Count == PageSize;
            });
        }

        [RelayCommand]
        private void ToggleSelection(WarehouseSelectableItem item)
        {
            if (!item.IsSelected) _selectedIdsSet.Add(item.Warehouse.Id);
            else _selectedIdsSet.Remove(item.Warehouse.Id);
        }

        [RelayCommand]
        private async Task SaveAndClose()
        {
            var result = _selectedIdsSet.ToList();
            await ShellService.GoBack(new Dictionary<string, object> { { "SelectedWarehouseIds", result } });
        }

        [RelayCommand] private async Task NextPage() { CurrentPage++; await LoadWarehouses(); }
        [RelayCommand] private async Task PreviousPage() { if (CurrentPage > 1) { CurrentPage--; await LoadWarehouses(); } }
        [RelayCommand] private async Task Cancel() => await ShellService.GoBack();
    }

    public partial class WarehouseSelectableItem : ObservableObject
    {
        public WarehouseResponse Warehouse { get; set; }
        [ObservableProperty] private bool isSelected;
    }
}
