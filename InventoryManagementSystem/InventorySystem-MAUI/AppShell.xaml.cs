using InventorySystem_MAUI.View;
using InventorySystem_MAUI.ViewModel;

namespace InventorySystem_MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel appShellViewModel)
        {
            InitializeComponent();
            BindingContext = appShellViewModel;
            Routing.RegisterRoute(nameof(AddressCreatePage), typeof(AddressCreatePage));
            Routing.RegisterRoute(nameof(CompanyCreatePage), typeof(CompanyCreatePage));
            Routing.RegisterRoute(nameof(UserCreatePage), typeof(UserCreatePage));

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));

            Routing.RegisterRoute(nameof(WarehouseDetailsPage), typeof(WarehouseDetailsPage));
            Routing.RegisterRoute(nameof(WarehouseReportPage), typeof(WarehouseReportPage));
            Routing.RegisterRoute(nameof(WarehousePickerPage), typeof(WarehousePickerPage));
        }
    }
}
