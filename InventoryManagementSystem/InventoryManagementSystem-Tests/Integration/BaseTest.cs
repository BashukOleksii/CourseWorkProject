using System.Net.Http.Headers;
using System.Net.Http.Json;
using InventorySystem_Shared.User;

public class BaseTest : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;

    private static string? _adminToken;
    private static string? _managerToken;

    public BaseTest(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();


    }

    protected async Task AuthenticateAsync(UserRole role)
    {
        string? token = role == UserRole.admin ? _adminToken : _managerToken;

        if (string.IsNullOrEmpty(token))
        {
            var loginInfo = role == UserRole.admin
                ? new UserLogin { Email = "admin@test.com", Password = "Password_555" }
                : new UserLogin { Email = "manager@test.com", Password = "Password_555" };

            var response = await Client.PostAsJsonAsync("api/auth/sign-in", loginInfo);
            var result = await response.Content.ReadFromJsonAsync<TokensDataResponse>(); 

            token = result?.AccessToken;

            if (role == UserRole.admin) _adminToken = token;
            else _managerToken = token;
        }

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}