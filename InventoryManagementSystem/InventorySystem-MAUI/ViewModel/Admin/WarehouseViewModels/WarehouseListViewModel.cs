using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.View;
using InventorySystem_Shared.Warehouse;
using System.Collections.ObjectModel;

namespace InventorySystem_MAUI.ViewModel
{
    public partial class WarehouseListViewModel : BaseViewModel
    {
        private readonly WarehouseService _warehouseService;
        private CancellationTokenSource _searchCts; 

        [ObservableProperty] private ObservableCollection<WarehouseResponse> warehouses = new();
        [ObservableProperty] private string searchText;
        [ObservableProperty] private int currentPage = 1;
        [ObservableProperty] private int pageSize = 5;
        [ObservableProperty] private bool canGoNext;

        public WarehouseListViewModel(WarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        async partial void OnSearchTextChanged(string value)
        {
            CurrentPage = 1;

            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();

            try
            {
                await Task.Delay(500, _searchCts.Token);
                await LoadWarehouses();
            }
            catch (TaskCanceledException) { }
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

                Warehouses = new ObservableCollection<WarehouseResponse>(result);
                CanGoNext = result.Count == PageSize;
            });
        }

        [RelayCommand]
        private async Task NextPage()
        {
            CurrentPage++;
            await LoadWarehouses();
        }

        [RelayCommand]
        private async Task PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadWarehouses();
            }
        }

        [RelayCommand]
        private async Task Delete(WarehouseResponse warehouse)
        {
            bool confirm = await Shell.Current.DisplayAlertAsync("Видалення", $"Ви впевнSені, що хочете видалити {warehouse.Name}?", "Так", "Ні");
            if (!confirm) return;

            await RunBusyTask(async () =>
            {
                await _warehouseService.DeleteWarehouse(warehouse.Id);
                Warehouses.Remove(warehouse); 
            });
        }

        [RelayCommand]
        private async Task AddWarehouse()
        {
            await ShellService.NavigateTo(nameof(WarehouseDetailsPage));
        }

        [RelayCommand]
        private async Task EditWarehouse(WarehouseResponse warehouse)
        {
            await ShellService.NavigateTo(nameof(WarehouseDetailsPage), new Dictionary<string, object> { { "Id", warehouse.Id} });
        }

        [RelayCommand]
        private async Task GoToReports()
        {
            await ShellService.NavigateTo(nameof(WarehouseReportPage));
        }
    }
}
