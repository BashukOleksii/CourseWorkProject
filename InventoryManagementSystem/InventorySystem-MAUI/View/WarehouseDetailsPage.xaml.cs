using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class WarehouseDetailsPage : ContentPage
{
	public WarehouseDetailsPage(WarehouseDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}