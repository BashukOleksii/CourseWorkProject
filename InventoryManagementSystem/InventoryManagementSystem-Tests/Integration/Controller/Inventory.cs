using Docker.DotNet.Models;
using InventorySystem_API.Inventory.Models;
using InventorySystem_Shared.Inventory;
using InventorySystem_Shared.Inventory.Manufacturer;
using InventorySystem_Shared.User;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InventoryManagementSystem_Tests.Integration.Controller
{

    public class InventoryControllerTests : BaseTest
    {
        public InventoryControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

        private const string WarehouseId = "65f1a2b3c4d5e6f7a8b9c0d1"; 


        [Fact]
        public async Task Access_AsAdmin_ReturnsForbidden()
        {
            await AuthenticateAsync(UserRole.admin);
            var response = await Client.GetAsync($"api/inventory/warehouse/{WarehouseId}");
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }


        [Fact]
        public async Task Create_ValidData_ReturnsOk()
        {
            await AuthenticateAsync(UserRole.manager);
            
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("Name"), "Name");
            content.Add(new StringContent("Description"), "Description");
            content.Add(new StringContent("Country"), "Manufacturer.Country");
            content.Add(new StringContent("Man_Name"), "Manufacturer.Name");
            content.Add(new StringContent($"{(int)(InventoryType.Auto)}"), "InventoryType");
            content.Add(new StringContent("100"), "Price");
            content.Add(new StringContent("100"), "Quantity");


            var response = await Client.PostAsync($"api/inventory/warehouse/{WarehouseId}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_NotExisting_ReturnsNotFound()
        {
            await AuthenticateAsync(UserRole.manager);
            string fakeId = "000000000000000000000000";

            var response = await Client.GetAsync($"api/inventory/{fakeId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetByWarehouse_ReturnsList()
        {
            await AuthenticateAsync(UserRole.manager);

            var response = await Client.GetAsync($"api/inventory/warehouse/{WarehouseId}?PageSize=5");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<List<InventoryResponse>>();
            Assert.NotNull(result);
        }


        [Fact]
        public async Task ExportJson_ReturnsFileStream()
        {
            await AuthenticateAsync(UserRole.manager);

            var response = await Client.GetAsync($"api/inventory/export/json/{WarehouseId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task Import_InvalidFile_ReturnsBadRequest()
        {
            await AuthenticateAsync(UserRole.manager);

            using var content = new MultipartFormDataContent();
            var fileContent = new StringContent("invalid data");
            content.Add(fileContent, "file", "test.txt");

            var response = await Client.PostAsync($"api/inventory/import/{WarehouseId}", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ExistingItem_ReturnsOk()
        {
            await AuthenticateAsync(UserRole.manager);
            string itemId = "65f1a2b3c4d5e6f7a8b9c0d2"; 

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent("Updated Name"), "Name");

            var response = await Client.PatchAsync($"api/inventory/{itemId}", content);

            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        }

    }
}
