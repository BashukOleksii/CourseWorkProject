using InventorySystem_API.Inventory.Models;
using InventorySystem_API.Inventory.Service;
using InventorySystem_API.Loging.Service;
using InventorySystem_API.Report.Service;
using InventorySystem_API.User.Services;
using InventorySystem_API.Warehouse.Service;
using InventorySystem_Shared.AddressClass;
using InventorySystem_Shared.Inventory;
using InventorySystem_Shared.User;
using InventorySystem_Shared.Warehouse;
using Moq;
using QuestPDF.Infrastructure;

namespace InventoryManagementSystem_Tests.Unit.API
{
    public class ReportServiceTests
    {
        private readonly Mock<IInventoryService> _inventoryServiceMock;
        private readonly Mock<IWarehouseService> _warehouseServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly ReportService _reportService;

        public ReportServiceTests()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            _inventoryServiceMock = new Mock<IInventoryService>();
            _warehouseServiceMock = new Mock<IWarehouseService>();
            _userServiceMock = new Mock<IUserService>();

            _reportService = new ReportService(
                _inventoryServiceMock.Object,
                _warehouseServiceMock.Object,
                _userServiceMock.Object
            );
        }

        [Fact]
        public async Task GetInventoryReport_ShouldThrowInvalidOperationException_WhenNoData()
        {
            var warehouseId = "w1";
            _inventoryServiceMock.Setup(s => s.Get(It.IsAny<InventoryQuery>(), warehouseId))
                .ReturnsAsync(new List<InventoryResponse>());

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _reportService.GetInventoryReport(new InventoryQuery(), warehouseId));
        }

        [Fact]
        public async Task GetInventoryReport_ShouldReturnPdfBytes_WhenDataExists()
        {
            var warehouseId = "w1";
            var data = new List<InventoryResponse>
            {
                new InventoryResponse { Id = "1", Name = "Item 1", Price = 100, Quantity = 2, InventoryType = InventoryType.Electronics }
            };

            _inventoryServiceMock.Setup(s => s.Get(It.IsAny<InventoryQuery>(), warehouseId))
                .ReturnsAsync(data);

            var result = await _reportService.GetInventoryReport(new InventoryQuery(), warehouseId);

            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }

        [Fact]
        public async Task GetWarehouseReport_ShouldThrowInvalidOperationException_WhenNoData()
        {
            var companyId = "c1";
            _warehouseServiceMock.Setup(s => s.Get(companyId, It.IsAny<WarehouseQuery>()))
                .ReturnsAsync(new List<WarehouseResponse>());

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _reportService.GetWarehouseReport(new WarehouseQuery(), companyId));
        }

        [Fact]
        public async Task GetWarehouseReport_ShouldReturnPdfBytes_AndCalculateEmployees()
        {
            var companyId = "c1";
            var warehouseData = new List<WarehouseResponse>
            {
                new WarehouseResponse { Id = "w1", Name = "Warehouse 1", Area = 150, Address = new Address() }
            };

            _warehouseServiceMock.Setup(s => s.Get(companyId, It.IsAny<WarehouseQuery>()))
                .ReturnsAsync(warehouseData);
            _userServiceMock.Setup(s => s.GetCountInWarehouse("w1"))
                .ReturnsAsync(5);

            var result = await _reportService.GetWarehouseReport(new WarehouseQuery(), companyId);

            Assert.NotNull(result);
            _userServiceMock.Verify(s => s.GetCountInWarehouse("w1"), Times.Once);
        }

        [Fact]
        public async Task GetUserReport_ShouldThrowInvalidOperationException_WhenNoUsers()
        {
            var companyId = "c1";
            _userServiceMock.Setup(s => s.Get(companyId, It.IsAny<UserQuery>(), null))
                .ReturnsAsync(new List<UserResponse>());

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _reportService.GetUserReport(new UserQuery(), companyId));
        }

        [Fact]
        public async Task GetUserReport_ShouldFetchWarehouseNames_ForManagers()
        {
            var companyId = "c1";
            var users = new List<UserResponse>
            {
                new UserResponse { Id = "u1", Name = "Manager", UserRole = UserRole.manager, WarehouseIds = new List<string> { "w1" } }
            };
            var warehouses = new List<WarehouseResponse>
            {
                new WarehouseResponse { Id = "w1", Name = "Main Warehouse" }
            };

            _userServiceMock.Setup(s => s.Get(companyId, It.IsAny<UserQuery>(), null))
                .ReturnsAsync(users);
            _warehouseServiceMock.Setup(s => s.GetByIds(It.IsAny<string[]>(), companyId))
                .ReturnsAsync(warehouses);

            var result = await _reportService.GetUserReport(new UserQuery(), companyId);

            Assert.NotNull(result);
            _warehouseServiceMock.Verify(s => s.GetByIds(It.Is<string[]>(ids => ids.Contains("w1")), companyId), Times.Once);
        }

        [Fact]
        public async Task GetUserReport_ShouldNotFetchWarehouses_IfOnlyAdminsExist()
        {
            var companyId = "c1";
            var users = new List<UserResponse>
            {
                new UserResponse { Id = "a1", Name = "Admin", UserRole = UserRole.admin, WarehouseIds = null }
            };

            _userServiceMock.Setup(s => s.Get(companyId, It.IsAny<UserQuery>(), null))
                .ReturnsAsync(users);
            _warehouseServiceMock.Setup(s => s.GetByIds(It.IsAny<string[]>(), companyId))
                .ReturnsAsync(new List<WarehouseResponse>());

            var result = await _reportService.GetUserReport(new UserQuery(), companyId);

            Assert.NotNull(result);
            _warehouseServiceMock.Verify(s => s.GetByIds(It.Is<string[]>(ids => ids.Length == 0), companyId), Times.Once);
        }
    }
}