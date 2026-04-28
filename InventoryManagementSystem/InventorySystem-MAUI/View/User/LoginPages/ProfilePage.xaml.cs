using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(ProfileViewModel profileViewModel)
	{
		InitializeComponent();
		BindingContext = profileViewModel; 
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		((ProfileViewModel)BindingContext).LoadData();
    }
}