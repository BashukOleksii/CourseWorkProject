using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.ViewModel;
using CommunityToolkit.Mvvm;
using InventorySystem_MAUI.View;

namespace InventorySystem_MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            #region API-Requests
            builder.Services.AddTransient<AuthHandler>();
            builder.Services.AddHttpClient("AuthClient", client =>
            {
                client.BaseAddress = new Uri(Conection.BaseURI);
            });
            builder.Services.AddHttpClient("APIClient", client =>
            {
                client.BaseAddress = new Uri(Conection.BaseURI);
            }).AddHttpMessageHandler<AuthHandler>();
            #endregion

            #region Service
            builder.Services.AddSingleton<UserContextService>();
            builder.Services.AddSingleton<AddressService>();
            builder.Services.AddSingleton<CompanyService>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ResetPasswordService>();
            builder.Services.AddSingleton<WarehouseService>();
            builder.Services.AddScoped<UserService>();
            #endregion

            #region AppShell
            builder.Services.AddSingleton<AppShellViewModel>();
            builder.Services.AddSingleton<AppShell>();
            #endregion

            #region ViewModel
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();

            builder.Services.AddTransient<AddressCreateViewModel>();
            builder.Services.AddTransient<CompanyCreateViewModel>();
            builder.Services.AddTransient<UserCreateViewModel>();


            builder.Services.AddTransient<WarehouseListViewModel>();
            builder.Services.AddTransient<WarehouseDetailsViewModel>();
            builder.Services.AddTransient<WarehouseReportViewModel>();
            builder.Services.AddTransient<WarehousePickerViewModel>();

            builder.Services.AddTransient<UserListViewModel>();
            builder.Services.AddTransient<UserCreateFromAdminViewModel>();
            builder.Services.AddTransient<UserDetailViewModel>();
            builder.Services.AddTransient<UserReportViewModel>();
            #endregion

            return builder.Build();
        }
    }
}
