using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class LogPage : ContentPage
{
	public LogPage(LogViewModel logViewModel)
	{
		InitializeComponent();
		BindingContext = logViewModel;
    }

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is LogViewModel logViewModel)
		{
			logViewModel.ApplyFiltersCommand.Execute(null);
		}
    }
}