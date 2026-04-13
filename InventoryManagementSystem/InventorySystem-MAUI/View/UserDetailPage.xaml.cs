using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class UserDetailPage : ContentPage
{
	public UserDetailPage(UserDetailViewModel userDetailViewModel)
	{
		InitializeComponent();
		BindingContext = userDetailViewModel;
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is UserDetailViewModel vm)
			await vm.LoadUserDetails();
        
    }

}