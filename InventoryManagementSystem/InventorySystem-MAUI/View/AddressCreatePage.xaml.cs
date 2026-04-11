using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class AddressCreatePage : ContentPage
{
	public AddressCreatePage(AddressCreateViewModel addressCreateViewModel)
	{
		InitializeComponent();
		BindingContext = addressCreateViewModel;
	}
}