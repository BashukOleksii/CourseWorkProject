using InventorySystem_API.User.Models;
using InventorySystem_Shared.User;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;
using System.Net.Http.Json;

namespace InventoryManagementSystem_Tests.Integration.Controller
{

    public class ResetPasswordIntegrationTests : BaseTest
    {
        public ResetPasswordIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task ResetPassword_Success()
        {
            var email = "admin@test.com";
            var db = (IMongoDatabase)Factory.Services.GetService(typeof(IMongoDatabase))!;

            var codesCollection = db.GetCollection<ResetPasswordModel>("PasswordResetCodes");

            var generateResponse = await Client.GetAsync($"api/reset?email={email}");
            Assert.Equal(HttpStatusCode.NoContent, generateResponse.StatusCode);

            
        }

        [Fact]
        public async Task ResetPassword_InvalidCode_ReturnsBadRequest()
        {
            var resetDto = new ResetPasswordDTO { Email = "admin@test.com", Password = "AnyPassword" };
            var wrongCode = "000000";

            var response = await Client.PostAsJsonAsync($"api/reset?code={wrongCode}", resetDto);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GenerateCode_NonExistentEmail_ReturnsBadRequest()
        {
            var response = await Client.GetAsync("api/reset?email=nonexistent@test.com");
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
