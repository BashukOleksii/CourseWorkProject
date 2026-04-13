namespace InventorySystem_MAUI.View;

public partial class WarehousePickerPage : ContentPage
{
	public WarehousePickerPage(WarehousePickerPage warehousePickerPage)
	{
		InitializeComponent();
		BindingContext = warehousePickerPage;
    }
}