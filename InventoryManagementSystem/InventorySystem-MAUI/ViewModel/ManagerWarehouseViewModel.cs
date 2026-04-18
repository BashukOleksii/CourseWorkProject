using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Warehouse;
using System.Collections.ObjectModel;

namespace InventorySystem_MAUI.ViewModel
{
    public partial class ManagerWarehouseViewModel : BaseViewModel
    {
        private readonly WarehouseService _warehouseService;
        private readonly UserContextService _userContext;

        [ObservableProperty] private ObservableCollection<WarehouseResponse> warehouses = new();
        [ObservableProperty] private bool isListEmpty;

        [ObservableProperty] private int currentPage = 1;
        [ObservableProperty] private int pageSize = 6;
        [ObservableProperty] private bool canGoNext;

        private List<WarehouseResponse> _allAvailableWarehouses = new();

        public ManagerWarehouseViewModel(WarehouseService warehouseService, UserContextService userContext)
        {
            _warehouseService = warehouseService;
            _userContext = userContext;
        }

        [RelayCommand]
        public async Task LoadManagerWarehouses()
        {
            await RunBusyTask(async () =>
            {
                var userWarehouseIds = _userContext.CurrentUser?.WarehouseIds;

                if (userWarehouseIds == null || !userWarehouseIds.Any())
                {
                    IsListEmpty = true;
                    return;
                }

                _allAvailableWarehouses = await _warehouseService.GetWarehousesByIds(userWarehouseIds);
                ApplyPagination();
                IsListEmpty = !_allAvailableWarehouses.Any();
            });
        }

        private void ApplyPagination()
        {
            var paged = _allAvailableWarehouses
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            Warehouses = new ObservableCollection<WarehouseResponse>(paged);
            CanGoNext = _allAvailableWarehouses.Count > (CurrentPage * PageSize);
        }

        [RelayCommand]
        private async Task SelectWarehouse(WarehouseResponse warehouse)
        {
            await Shell.Current.DisplayAlertAsync("Success","Selected warehouse: " + warehouse.Name, "OK");
        }

        [RelayCommand]
        private void NextPage() { CurrentPage++; ApplyPagination(); }

        [RelayCommand]
        private void PreviousPage() { if (CurrentPage > 1) { CurrentPage--; ApplyPagination(); } }
    }
}