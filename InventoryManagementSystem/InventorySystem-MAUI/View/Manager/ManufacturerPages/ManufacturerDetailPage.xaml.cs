using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class ManufacturerDetailPage : ContentPage
{
	public ManufacturerDetailPage(ManufacturerDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}