using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel loginViewModel)
	{
		InitializeComponent();
		BindingContext = loginViewModel;
	}
}