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
            Routing.RegisterRoute(nameof(WelcomePage), typeof(WelcomePage));
            Routing.RegisterRoute(nameof(AddressCreatePage), typeof(AddressCreatePage));
            Routing.RegisterRoute(nameof(CompanyCreatePage), typeof(CompanyCreatePage));
        }
    }
}
