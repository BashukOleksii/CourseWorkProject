using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class WarehouseListPage : ContentPage
{
	public WarehouseListPage(WarehouseListViewModel warehouseListViewModel)
	{
		InitializeComponent();
		BindingContext = warehouseListViewModel;
	}
}