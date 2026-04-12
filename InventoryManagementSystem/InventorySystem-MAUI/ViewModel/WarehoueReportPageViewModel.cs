using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Warehouse;
using System.Collections.ObjectModel;

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
    }

    [RelayCommand]
    private void ToggleFilters() => IsFilterVisible = !IsFilterVisible;

    [RelayCommand]
    public async Task ApplyFilters()
    {
        Query.Page = CurrentPage;
        await RunBusyTask(async () =>
        {
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
            if (pdfData != null)
            {
                var fileName = $"Warehouses_{DateTime.Now:yyyyMMdd}.pdf";
                var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
                await File.WriteAllBytesAsync(filePath, pdfData);

                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
        });
    }

    [RelayCommand]
    private async Task NextPage() { CurrentPage++; await ApplyFilters(); }

    [RelayCommand]
    private async Task PreviousPage() { if (CurrentPage > 1) { CurrentPage--; await ApplyFilters(); } }
}