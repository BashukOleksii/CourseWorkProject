using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class UserListPage : ContentPage
{
	public UserListPage(UserListViewModel userListViewModel)
	{
		InitializeComponent();
		BindingContext = userListViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is UserListViewModel vm)
            await vm.LoadUsers();
        
    }
}