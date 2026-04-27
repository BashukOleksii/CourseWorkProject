using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class InventoryAggregationPage : ContentPage
{
	public InventoryAggregationPage(InventoryAggregationViewModel inventoryAggregationViewModel)
	{
		InitializeComponent();
		BindingContext = inventoryAggregationViewModel;
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is InventoryAggregationViewModel viewModel)
			await viewModel.LoadData();
		
    }
}