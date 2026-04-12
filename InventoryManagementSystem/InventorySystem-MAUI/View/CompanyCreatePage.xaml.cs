using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI.View;

public partial class CompanyCreatePage : ContentPage
{
    public CompanyCreatePage(CompanyCreateViewModel companyCreateViewModel)
    {
        InitializeComponent();
        BindingContext = companyCreateViewModel;
    }
}