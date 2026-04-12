using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.View;
using InventorySystem_Shared.Warehouse;
using System.Collections.ObjectModel;

namespace InventorySystem_MAUI.ViewModel
{
public partial class WarehouseReportViewModel : BaseViewModel
{
    private readonly WarehouseService _warehouseService;

    [ObservableProperty] private ObservableCollection<WarehouseResponse> warehouses = new();
    [ObservableProperty] private WarehouseQuery query = new();
    [ObservableProperty] private bool isFilterVisible = false;

    [ObservableProperty] private int currentPage = 1;
    [ObservableProperty] private bool canGoNext;

    public WarehouseReportViewModel(WarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
        Task.Run(async () => await ApplyFilters());
    }

    [RelayCommand]
    private void ToggleFilters() => IsFilterVisible = !IsFilterVisible;

    [RelayCommand]
    public async Task ApplyFilters()
    {
        await RunBusyTask(async () =>
        {
            Query.Page = CurrentPage;

            var result = await _warehouseService.GetWarehouses(Query);

            Warehouses = new ObservableCollection<WarehouseResponse>(result);
            CanGoNext = result.Count == Query.PageSize;
            IsFilterVisible = false;
        });
    }

    [RelayCommand]
    private async Task GeneratePdfReport()
    {
        await RunBusyTask(async () =>
        {
            var pdfData = await _warehouseService.GetWarehouseReport(Query);


            var fileName = $"Warehouse_Report_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            await File.WriteAllBytesAsync(filePath, pdfData);

            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath),
                Title = "Перегляд звіту"
            });

        });
    }

    [RelayCommand]
    private async Task NextPage()
    {
        CurrentPage++;
        await ApplyFilters();
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
    private async Task EditWarehouse(WarehouseResponse warehouse)
    {
        await ShellService.NavigateTo(nameof(WarehouseDetailsPage), new Dictionary<string, object> { { "Id", warehouse.Id } });
    }

    [RelayCommand]
    private async Task PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await ApplyFilters();
        }
    }
}