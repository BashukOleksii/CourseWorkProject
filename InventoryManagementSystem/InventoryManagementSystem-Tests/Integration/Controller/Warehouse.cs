using System.Net;
using System.Net.Http.Json;
using InventorySystem_Shared.Warehouse;
using InventorySystem_Shared.User;
using InventorySystem_Shared.AddressClass;
using Xunit;

namespace InventoryManagementSystem_Tests.Integration.Controller
{

    public class WarehouseControllerTests : BaseTest
    {
        public WarehouseControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

        private WarehouseDTO GetTestWarehouseDto(string suffix = "") => new WarehouseDTO
        {
            Name = $"Main Warehouse {suffix}",
            Description = "Primary storage facility",
            Area = 500.5,
            Address = new Address { City = "Khmelnytskyi", Street = "Industrialna", HouseNumber = "10" }
        };


        [Fact]
        public async Task Create_AsAdmin_ReturnsOkAndCreatedWarehouse()
        {
            await AuthenticateAsync(UserRole.admin);
            var dto = GetTestWarehouseDto();

            var response = await Client.PostAsJsonAsync("api/warehouse", dto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<WarehouseDTO>();
            Assert.NotNull(created);
            Assert.Equal(dto.Name, created.Name);
        }

        [Fact]
        public async Task Create_AsManager_ReturnsForbidden()
        {
            await AuthenticateAsync(UserRole.manager);
            var dto = GetTestWarehouseDto("Manager");

            var response = await Client.PostAsJsonAsync("api/warehouse", dto);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }


        [Fact]
        public async Task GetWarehouses_ReturnsSuccessList()
        {
            await AuthenticateAsync(UserRole.admin);
            await Client.PostAsJsonAsync("api/warehouse", GetTestWarehouseDto("ListTest"));

            var response = await Client.GetAsync("api/warehouse?PageSize=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var warehouses = await response.Content.ReadFromJsonAsync<List<WarehouseDTO>>();
            Assert.NotNull(warehouses);
            Assert.NotEmpty(warehouses);
        }

        [Fact]
        public async Task GetById_WarehouseNotExists_ReturnsNotFound()
        {
            await AuthenticateAsync(UserRole.manager);
            string fakeId = "65f1a2b3c4d5e6f7a8b9c0d1"; 

            var response = await Client.GetAsync($"api/warehouse/{fakeId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Fact]
        public async Task Update_AsAdmin_ReturnsUpdatedWarehouse()
        {
            await AuthenticateAsync(UserRole.admin);

            var createRes = await Client.PostAsJsonAsync("api/warehouse", GetTestWarehouseDto("ToUpdate"));

            var createdWarehouse = await createRes.Content.ReadFromJsonAsync<WarehouseResponse>();
            string warehouseId = createdWarehouse.Id;

            var updateDto = new WarehouseUpdate { Name = "Updated Warehouse Name" };

            var response = await Client.PutAsJsonAsync($"api/warehouse/{warehouseId}", updateDto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var updated = await response.Content.ReadFromJsonAsync<WarehouseDTO>();
            Assert.Equal("Updated Warehouse Name", updated?.Name);
        }

        [Fact]
        public async Task Update_AsManager_ReturnsForbidden()
        {
            await AuthenticateAsync(UserRole.manager);
            var updateDto = new WarehouseUpdate { Name = "Illegal Update" };

            var response = await Client.PutAsJsonAsync("api/warehouse/some-id", updateDto);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Delete_AsAdmin_ReturnsNoContent()
        {
            await AuthenticateAsync(UserRole.admin);
            var createRes = await Client.PostAsJsonAsync("api/warehouse", GetTestWarehouseDto("ToDelete"));
            var createdWarehouse = await createRes.Content.ReadFromJsonAsync<WarehouseResponse>();
            string warehouseId = createdWarehouse.Id;

            var response = await Client.DeleteAsync($"api/warehouse/{warehouseId}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Delete_AsManager_ReturnsForbidden()
        {
            await AuthenticateAsync(UserRole.manager);

            var response = await Client.DeleteAsync("api/warehouse/some-id");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

    }

}
