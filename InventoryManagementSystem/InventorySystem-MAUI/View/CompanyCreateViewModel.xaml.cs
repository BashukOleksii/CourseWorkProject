namespace InventorySystem_MAUI.View;

public partial class CompanyCreateViewModel : ContentPage
{
	public CompanyCreateViewModel(CompanyCreateViewModel companyCreateViewModel)
	{
		InitializeComponent();
		BindingContext = companyCreateViewModel;
	}
}