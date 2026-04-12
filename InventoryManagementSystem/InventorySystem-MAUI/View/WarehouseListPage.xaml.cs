using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class WarehouseListPage : ContentPage
{
    public WarehouseListPage(WarehouseListViewModel warehouseListViewModel)
    {
        InitializeComponent();
        BindingContext = warehouseListViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as WarehouseListViewModel;
        if (vm != null)
            await vm.LoadWarehouses();
        

    }
}