using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class CompanyPage : ContentPage
{
	public CompanyPage(CompanyViewModel companyViewModel)
	{
		InitializeComponent();
		BindingContext = companyViewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is CompanyViewModel vm)
			await vm.LoadCompany();
		
    }
}