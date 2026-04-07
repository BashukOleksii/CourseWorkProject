using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.ViewModel;

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

            builder.Services.AddSingleton<UserContextService>();

            #region API-Requests
            builder.Services.AddHttpClient("AuthClient", client =>
            {
                client.BaseAddress = new Uri(Conection.BaseURI);
            });
            builder.Services.AddHttpClient("APIClient", client =>
            {
                client.BaseAddress = new Uri(Conection.BaseURI);
            }).AddHttpMessageHandler<AuthHandler>();
            #endregion

            builder.Services.AddSingleton<AppShellViewModel>();
            builder.Services.AddSingleton<AppShell>();

            return builder.Build();
        }
    }
}
