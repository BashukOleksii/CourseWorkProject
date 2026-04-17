using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class ManufacturerListPage : ContentPage
{
	public ManufacturerListPage(ManufacturerListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ManufacturerListViewModel vm)
            await vm.LoadData();

    }
}