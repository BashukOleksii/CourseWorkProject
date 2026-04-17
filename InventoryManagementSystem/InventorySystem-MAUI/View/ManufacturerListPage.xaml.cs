using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class ManufacturerListPage : ContentPage
{
	public ManufacturerListPage(ManufacturerListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}