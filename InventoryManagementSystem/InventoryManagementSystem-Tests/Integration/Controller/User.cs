using System.Net;
using System.Net.Http.Json;
using InventorySystem_Shared.User;
using MongoDB.Driver;
using InventorySystem_API.User.Model;
using Microsoft.AspNetCore.Http;

namespace InventoryManagementSystem_Tests.Integration.Controller
{

    public class UserIntegrationTests : BaseTest
    {
        public UserIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

        private async Task<UserModel> GetUserFromDb(string email)
        {
            var db = (IMongoDatabase)Factory.Services.GetService(typeof(IMongoDatabase))!;
            return await db.GetCollection<UserModel>("Users").Find(u => u.Email == email).FirstOrDefaultAsync();
        }


        [Fact]
        public async Task GetById_AdminRequestsSelf_ReturnsOk()
        {
            await AuthenticateAsync(UserRole.admin);
            var userInDb = await GetUserFromDb("admin@test.com");

            var response = await Client.GetAsync($"api/user/{userInDb.Id}");

            response.EnsureSuccessStatusCode();
            var user = await response.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(user);
            Assert.Equal(userInDb.Email, user.Email);
        }

        [Fact]
        public async Task GetById_ManagerRequests_ReturnsForbidden()
        {
            await AuthenticateAsync(UserRole.manager);
            var userInDb = await GetUserFromDb("admin@test.com");

            var response = await Client.GetAsync($"api/user/{userInDb.Id}");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetById_NonExistentId_ReturnsNotFound()
        {
            await AuthenticateAsync(UserRole.admin);
            var fakeId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            var response = await Client.GetAsync($"api/user/{fakeId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Get_AdminRequestsAll_ReturnsList()
        {
            await AuthenticateAsync(UserRole.admin);

            var response = await Client.GetAsync("api/user?PageSize=10&Page=1");
            
            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
            Assert.NotNull(users);
            Assert.True(users.Count >= 1); 
        }

        [Fact]
        public async Task Update_ValidData_ReturnsOk()
        {
            await AuthenticateAsync(UserRole.admin);
            var userInDb = await GetUserFromDb("manager@test.com");

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent("Updated Name"), "Name");
            content.Add(new StringContent(UserRole.manager.ToString()), "UserRole");
            
            var response = await Client.PatchAsync($"api/user/{userInDb.Id}", content);

            response.EnsureSuccessStatusCode();
            var updatedUser = await response.Content.ReadFromJsonAsync<UserResponse>();
            Assert.Equal("Updated Name", updatedUser?.Name);
        }

        [Fact]
        public async Task AddWarehouses_ValidIds_ReturnsNoContent()
        {
            await AuthenticateAsync(UserRole.admin);
            var userInDb = await GetUserFromDb("manager@test.com");
            var warehouses = new[] { "wh_1", "wh_2" };

            var response = await Client.PatchAsJsonAsync($"api/user/{userInDb.Id}/warehouses", warehouses);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

       
        [Fact]
        public async Task Delete_AdminDeletesManager_ReturnsNoContent()
        {
            await AuthenticateAsync(UserRole.admin);
            var userInDb = await GetUserFromDb("manager@test.com");

            var response = await Client.DeleteAsync($"api/user/{userInDb.Id}");
         
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var checkUser = await GetUserFromDb("manager@test.com");
            Assert.Null(checkUser);
        }


        [Fact]
        public async Task WhoAmI_AuthenticatedUser_ReturnsCurrentUserInfo()
        {
            await AuthenticateAsync(UserRole.manager);

            var response = await Client.GetAsync("api/user/whoami");

            response.EnsureSuccessStatusCode();
            var user = await response.Content.ReadFromJsonAsync<UserResponse>();
            Assert.Equal("manager@test.com", user?.Email);
        }

        [Fact]
        public async Task WhoAmI_Unauthenticated_ReturnsUnauthorized()
        {
            ClearAuthentication();

            var response = await Client.GetAsync("api/user/whoami");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

    }
}
