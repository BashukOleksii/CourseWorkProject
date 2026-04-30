using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View.Manager.InventoryPages;

public partial class OrderCreationPage : ContentPage
{
	public OrderCreationPage(OrderCreationViewModel orderCreationViewModel)
	{
		InitializeComponent();
		BindingContext = orderCreationViewModel;
    }
}