using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class InventoryListPage : ContentPage
{
	public InventoryListPage(InventoryListViewModel inventoryListViewModel)
	{
		InitializeComponent();
		BindingContext = inventoryListViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is InventoryListViewModel vm)
            await vm.LoadItems();
    }
}