using InventorySystem_Shared.User;
using System.Net;
using System.Net.Http.Json;

public class AuthIntegrationTest : BaseTest
{
    public AuthIntegrationTest(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task SignUp_And_SignIn_FullFlow_Test()
    {
        var registrationData = new MultipartFormDataContent();
        registrationData.Add(new StringContent("test-company-id"), "CompanyId");
        registrationData.Add(new StringContent("Admin User"), "Name");
        registrationData.Add(new StringContent("admin@test.com"), "Email");
        registrationData.Add(new StringContent("admin"), "UserRole");
        registrationData.Add(new StringContent("Password123!"), "Password");

        var signUpResponse = await Client.PostAsync("api/auth/sign-up", registrationData);

        Assert.Equal(HttpStatusCode.OK, signUpResponse.StatusCode);

        var loginData = new UserLogin
        {
            Email = "admin@test.com",
            Password = "Password123!"
        };

        var signInResponse = await Client.PostAsJsonAsync("api/auth/sign-in", loginData);

        Assert.Equal(HttpStatusCode.OK, signInResponse.StatusCode);
        var authResult = await signInResponse.Content.ReadFromJsonAsync<TokensDataResponse>();

        Assert.NotNull(authResult?.AccessToken);
        Assert.NotNull(authResult?.RefreshToken);
    }

    [Fact]
    public async Task SignIn_ShouldReturnBadRequest_WithWrongPassword()
    {
        var loginData = new UserLogin { Email = "admin@test.com", Password = "WrongPassword" };
        var response = await Client.PostAsJsonAsync("api/auth/sign-in", loginData);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}