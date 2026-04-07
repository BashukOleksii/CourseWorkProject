namespace InventorySystem_MAUI.View;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
	}

	public async void OnGetStartedClicked(object sender, EventArgs e)
	{
		await Shell.Current.DisplayAlertAsync("Кнопка","Перехід на сторінку входу","OK");
    }
}