using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View.Manager.InventoryPages;

public partial class OrderInventoryPickerPage : ContentPage
{
	public OrderInventoryPickerPage(OrderInventoryPickerViewModel orderInventory)
	{
		InitializeComponent();
		BindingContext = orderInventory;
    }

	protected async override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is OrderInventoryPickerViewModel viewModel)
		{
			await viewModel.LoadData();
        }
    }
}