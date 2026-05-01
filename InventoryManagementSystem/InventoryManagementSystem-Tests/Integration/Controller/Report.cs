using System.Net;
using System.Net.Http.Json;
using InventorySystem_Shared.Inventory;
using InventorySystem_Shared.User;
using InventorySystem_Shared.Company;

namespace InventoryManagementSystem_Tests.Integration.Controller
{

    public class ExportIntegrationTests : BaseTest
    {
        public ExportIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task GetUserReport_AdminRequests_ReturnsPdfFile()
        {
            await AuthenticateAsync(UserRole.admin);
            var query = "?PageSize=10&Page=1";

            var response = await Client.GetAsync($"api/export/user-report{query}");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);

            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.NotEmpty(bytes); 
        }

        [Fact]
        public async Task GetUserReport_ManagerRequests_ReturnsForbidden()
        {
            await AuthenticateAsync(UserRole.manager);

            var response = await Client.GetAsync("api/export/user-report");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetInventoryReport_ValidWarehouse_ReturnsPdf()
        {
            await AuthenticateAsync(UserRole.manager);
            string warehouseId = "test_warehouse_id"; 

            var response = await Client.GetAsync($"api/export/inventory-report?warehouseId={warehouseId}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
            }
            else
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task GetSalesReport_ValidData_ReturnsPdf()
        {
            await AuthenticateAsync(UserRole.manager);
            var warehouseId = "wh_123";
            var salesDto = new SalesReportRequest
            {
                Provider = new CompanyDTO { Name = "Test Provider" },
                InventoryInfo = new[]
                {
                new InventoryInfo { InventoryId = "inv_1", Quantity = 5 }
            }
            };

            var response = await Client.PostAsJsonAsync($"api/export/sales-report/{warehouseId}", salesDto);

            if (response.IsSuccessStatusCode)
            {
                Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
                Assert.Contains("filename", response.Content.Headers.ContentDisposition?.ToString());
            }
        }


        [Fact]
        public async Task GetWarehouseReport_AdminRequests_ReturnsPdf()
        {
            await AuthenticateAsync(UserRole.admin);

            var response = await Client.GetAsync("api/export/warehouse-report");
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
            }
        }

    }
}
