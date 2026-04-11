using InventorySystem_MAUI.Helper;

namespace InventorySystem_MAUI.View;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
	}

	public async void OnGetStartedClicked(object sender, EventArgs e)
	{
		await ShellService.NavigateTo(nameof(LoginPage));
    }
}