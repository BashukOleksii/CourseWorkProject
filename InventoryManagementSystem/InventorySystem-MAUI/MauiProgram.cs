using InventorySystem_MAUI.Helper;

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

            return builder.Build();
        }
    }
}
