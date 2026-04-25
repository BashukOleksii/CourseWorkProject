using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class WarehousePickerPage : ContentPage
{
	public WarehousePickerPage(WarehousePickerViewModel warehousePickerViewModel)
	{
		InitializeComponent();
		BindingContext = warehousePickerViewModel;
    }
}