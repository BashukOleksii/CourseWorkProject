using InventorySystem_Shared.Loging;
using InventorySystem_Shared.User;
using MongoDB.Driver;
using System.Net;
using System.Net.Http.Json;

namespace InventoryManagementSystem_Tests.Integration.Controller
{


    public class AuditLogIntegrationTests : BaseTest
    {
        public AuditLogIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task GetLogs_AdminRequests_ReturnsOk()
        {
            await AuthenticateAsync(UserRole.admin);

            var response = await Client.GetAsync("api/auditlog?PageSize=10");

            response.EnsureSuccessStatusCode();
            var logs = await response.Content.ReadFromJsonAsync<List<AuditLogResponse>>();
            Assert.NotNull(logs);
        }

        [Fact]
        public async Task GetLogs_ManagerRequests_ReturnsForbidden()
        {
            await AuthenticateAsync(UserRole.manager);

            var response = await Client.GetAsync("api/auditlog");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
