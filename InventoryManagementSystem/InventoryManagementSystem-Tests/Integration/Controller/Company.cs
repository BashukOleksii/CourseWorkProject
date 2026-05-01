using System.Net;
using System.Net.Http.Json;
using InventorySystem_Shared.Company;
using InventorySystem_Shared.User;
using InventorySystem_Shared.AddressClass;

namespace InventoryManagementSystem_Tests.Integration.Controller
{

    public class CompanyControllerTests : BaseTest
    {
        public CompanyControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task GetMyCompany_AsAdmin_ReturnsSuccessAndCorrectCompany()
        {
            await AuthenticateAsync(UserRole.admin);

            var response = await Client.GetAsync("api/company");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var company = await response.Content.ReadFromJsonAsync<CompanyDTO>();
            Assert.NotNull(company);
        }

        [Fact]
        public async Task GetMyCompany_AsManager_ReturnsSuccess()
        {
            await AuthenticateAsync(UserRole.manager);

            var response = await Client.GetAsync("api/company");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMyCompany_Unauthorized_ReturnsUnauthorized()
        {
            var response = await Client.GetAsync("api/company");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Fact]
        public async Task CreateCompany_ValidData_ReturnsOk()
        {
            var newCompany = new CompanyDTO
            {
                Name = "New Integration Company",
                Description = "Description",
                Phone = "+380123456789",
                Address = new Address { City = "Khmelnytskyi", Street = "Main St" }
            };

            var response = await Client.PostAsJsonAsync("api/company", newCompany);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<CompanyDTO>();
            Assert.Equal(newCompany.Name, result?.Name);
        }

        [Fact]
        public async Task UpdateMyCompany_AsAdmin_ReturnsBadRequest()
        {
            await AuthenticateAsync(UserRole.admin);
            var updateInfo = new CompanyUpdate
            {
                Name = ""
            };

            var response = await Client.PatchAsJsonAsync("api/company", updateInfo);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var updated = await response.Content.ReadFromJsonAsync<CompanyDTO>();
            Assert.Equal("", updated?.Name);
        }

        [Fact]
        public async Task UpdateMyCompany_AsManager_ReturnsForbidden()
        {
            await AuthenticateAsync(UserRole.manager); 
            var updateInfo = new CompanyUpdate { Name = "Hack Attempt" };

            var response = await Client.PatchAsJsonAsync("api/company", updateInfo);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

    }
}
