using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class UserCreatePage : ContentPage
{
	public UserCreatePage(UserCreateViewModel userCreateViewModel)
	{
		InitializeComponent();
		BindingContext = userCreateViewModel;
	}
}