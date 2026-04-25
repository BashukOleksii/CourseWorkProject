using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class UserCreateFromAdminPage : ContentPage
{
	public UserCreateFromAdminPage(UserCreateFromAdminViewModel userCreateFromAdminViewModel)
	{
		InitializeComponent();
		BindingContext = userCreateFromAdminViewModel;
    }
}