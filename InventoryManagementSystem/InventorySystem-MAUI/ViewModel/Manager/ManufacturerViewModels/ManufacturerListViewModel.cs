using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.View;
using InventorySystem_Shared.Inventory.Manufacturer;
using System.Collections.ObjectModel;

namespace InventorySystem_MAUI.ViewModel;
public partial class ManufacturerListViewModel : BaseViewModel
{
    private readonly ManufacturerService _service;
    private List<InventoryManufacturer> _fullList = new();

    [ObservableProperty] private ObservableCollection<InventoryManufacturer> displayedManufacturers = new();
    [ObservableProperty] private string searchText;
    [ObservableProperty] private bool isListEmpty;

    [ObservableProperty] private int currentPage = 1;
    [ObservableProperty] private int pageSize = 10;
    [ObservableProperty] private bool canGoNext;

    public ManufacturerListViewModel(ManufacturerService service)
    {
        _service = service;
    }

    [RelayCommand]
    public async Task LoadData()
    {
        await RunBusyTask(async () =>
        {
            _fullList = await _service.GetManufacturersAsync();
            ApplyFilters();
        });
    }

    async partial void OnSearchTextChanged(string value)
    {
        CurrentPage = 1; 
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = _fullList.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(m => m.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                          m.Country.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        var list = filtered.ToList();
        IsListEmpty = !list.Any();

        CanGoNext = list.Count > CurrentPage * PageSize;
        var paged = list.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

        DisplayedManufacturers = new ObservableCollection<InventoryManufacturer>(paged);
    }

    [RelayCommand]
    private async Task AddManufacturer()
    {
        await ShellService.NavigateTo(nameof(ManufacturerDetailPage));
    }

    [RelayCommand]
    private async Task Edit(InventoryManufacturer manufacturer)
    {
        var parameters = new Dictionary<string, object>
    {
        { "Manufacturer", manufacturer }
    };
        await Shell.Current.GoToAsync(nameof(ManufacturerDetailPage), parameters);
    }


    [RelayCommand]
    private async Task Delete(InventoryManufacturer item)
    {
        if (await Shell.Current.DisplayAlertAsync("Видалення", $"Видалити {item.Name}?", "Так", "Ні"))
        {
            _fullList.Remove(item);
            await _service.SaveManufacturersAsync(_fullList);
            ApplyFilters();
        }
    }

    [RelayCommand] private void NextPage() { CurrentPage++; ApplyFilters(); }
    [RelayCommand] private void PreviousPage() { if (CurrentPage > 1) { CurrentPage--; ApplyFilters(); } }
}