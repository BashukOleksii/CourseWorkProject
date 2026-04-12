using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class WarehouseReportPage : ContentPage
{
	public WarehouseReportPage(WarehouseReportViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}