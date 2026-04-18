using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class ManagerWarehousePage : ContentPage
{
	public ManagerWarehousePage(ManagerWarehouseViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is ManagerWarehouseViewModel viewModel)
			await viewModel.LoadManagerWarehouses();
    }
}