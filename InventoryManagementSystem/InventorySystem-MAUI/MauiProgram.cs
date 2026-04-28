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
            builder.Services.AddSingleton<IUserContextService,UserContextService>();
            builder.Services.AddSingleton<IAddressService, AddressService>();
            builder.Services.AddSingleton<ICompanyService,CompanyService>();
            builder.Services.AddSingleton<IAuthService,AuthService>();
            builder.Services.AddSingleton<IResetPasswordService,ResetPasswordService>();
            builder.Services.AddSingleton<IWarehouseService ,WarehouseService>();
            builder.Services.AddSingleton<IUserService,UserService>();
            builder.Services.AddSingleton<ILogService, LogService>();
            builder.Services.AddSingleton<IManufacturerService,ManufacturerService>();
            builder.Services.AddSingleton<IInventoryService,InventoryService>();
            #endregion

            #region AppShell
            builder.Services.AddSingleton<AppShellViewModel>();
            builder.Services.AddSingleton<AppShell>();
            #endregion

            #region ViewModel

            #region User
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();

            builder.Services.AddTransient<AddressCreateViewModel>();
            builder.Services.AddTransient<CompanyCreateViewModel>();
            builder.Services.AddTransient<UserCreateViewModel>();
            #endregion

            #region Admin
            builder.Services.AddTransient<WarehouseListViewModel>();
            builder.Services.AddTransient<WarehouseDetailsViewModel>();
            builder.Services.AddTransient<WarehouseReportViewModel>();
            builder.Services.AddTransient<WarehousePickerViewModel>();

            builder.Services.AddTransient<UserListViewModel>();
            builder.Services.AddTransient<UserCreateFromAdminViewModel>();
            builder.Services.AddTransient<UserDetailViewModel>();
            builder.Services.AddTransient<UserReportViewModel>();

            builder.Services.AddScoped<CompanyViewModel>();

            builder.Services.AddTransient<LogViewModel>();
            #endregion

            #region Manager
            builder.Services.AddTransient<ManufacturerListViewModel>();
            builder.Services.AddTransient<ManufacturerDetailsViewModel>();

            builder.Services.AddTransient<ManagerWarehouseViewModel>();

            builder.Services.AddTransient<InventoryListViewModel>();
            builder.Services.AddTransient<InventoryDetailsViewModel>();
            builder.Services.AddTransient<InventoryAggregationViewModel>();
            #endregion

            #endregion

            return builder.Build();
        }
    }
}
