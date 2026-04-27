using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class ForgotPasswordPage : ContentPage
{
	public ForgotPasswordPage(ForgotPasswordViewModel forgotPasswordViewModel)
	{
		InitializeComponent();
		BindingContext = forgotPasswordViewModel;
	}
}