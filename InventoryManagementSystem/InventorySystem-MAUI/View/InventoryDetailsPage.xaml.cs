using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class InventoryDetailsPage : ContentPage
{
	public InventoryDetailsPage(InventoryDetailsViewModel inventoryDetailsViewModel)
	{
		InitializeComponent();
        BindingContext = inventoryDetailsViewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is InventoryDetailsViewModel viewModel)
            await viewModel.Initialize();
    }
}