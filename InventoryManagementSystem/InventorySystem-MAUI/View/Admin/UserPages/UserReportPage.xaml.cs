using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class UserReportPage : ContentPage
{
	public UserReportPage(UserReportViewModel userReportViewModel)
	{
		InitializeComponent();
		BindingContext = userReportViewModel;
	}
}