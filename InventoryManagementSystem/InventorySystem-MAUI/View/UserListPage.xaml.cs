using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class UserListPage : ContentPage
{
	public UserListPage(UserListViewModel userListViewModel)
	{
		InitializeComponent();
		BindingContext = userListViewModel;
    }
}