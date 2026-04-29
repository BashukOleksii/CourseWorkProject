using Moq;
using AutoMapper;
using InventorySystem_API.Warehouse.Service;
using InventorySystem_API.Warehouse.Repository;
using InventorySystem_API.Warehouse.Models;
using InventorySystem_API.Warehouse.Validation;
using InventorySystem_API.User.Services;
using InventorySystem_API.Inventory.Service;
using InventorySystem_Shared.Warehouse;
using InventorySystem_Shared.AddressClass;
using MongoDB.Driver;

namespace InventoryManagementSystem_Tests.Unit.API
{
    public class WarehouseServiceTests
    {
        private readonly Mock<IWarehouseRepository> _warehouseRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IInventoryService> _inventoryServiceMock;
        private readonly WarehouseValidator _validator;
        private readonly WarehouseService _warehouseService;

        public WarehouseServiceTests()
        {
            _warehouseRepoMock = new Mock<IWarehouseRepository>();
            _mapperMock = new Mock<IMapper>();
            _userServiceMock = new Mock<IUserService>();
            _inventoryServiceMock = new Mock<IInventoryService>();
            _validator = new WarehouseValidator();

            _warehouseService = new WarehouseService(
                _warehouseRepoMock.Object,
                _userServiceMock.Object,
                _mapperMock.Object,
                _validator,
                _inventoryServiceMock.Object
            );
        }
        [Fact]
        public async Task GetById_ShouldReturnWarehouse_WhenExistsAndCompanyMatches()
        {
            var id = "w1";
            var companyId = "c1";
            var model = new WarehouseModel { Id = id, CompanyId = companyId };
            var response = new WarehouseResponse { Id = id };

            _warehouseRepoMock.Setup(r => r.GetById(id)).ReturnsAsync(model);
            _mapperMock.Setup(m => m.Map<WarehouseResponse>(model)).Returns(response);

            var result = await ((IWarehouseService)_warehouseService).GetById(id, companyId);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetById_ShouldThrowArgumentException_WhenCompanyMismatch()
        {
            var id = "w1";
            var model = new WarehouseModel { Id = id, CompanyId = "other_company" };
            _warehouseRepoMock.Setup(r => r.GetById(id)).ReturnsAsync(model);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                ((IWarehouseService)_warehouseService).GetById(id, "my_company"));
        }

        [Fact]
        public async Task Create_ShouldReturnResponse_WhenDataIsValid()
        {
            var dto = new WarehouseDTO { Name = "Main" };
            var companyId = "c1";
            var model = new WarehouseModel { Name = "Main" };
            var response = new WarehouseResponse { Name = "Main" };

            _mapperMock.Setup(m => m.Map<WarehouseModel>(dto)).Returns(model);
            _warehouseRepoMock.Setup(r => r.Create(model)).ReturnsAsync(model);
            _mapperMock.Setup(m => m.Map<WarehouseResponse>(model)).Returns(response);

            var result = await _warehouseService.Create(dto, companyId);

            Assert.Equal(companyId, model.CompanyId);
            Assert.NotNull(result);
            _warehouseRepoMock.Verify(r => r.Create(It.IsAny<WarehouseModel>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldInvokeDependencies_WhenWarehouseExists()
        {
            var id = "w1";
            var companyId = "c1";
            var model = new WarehouseModel { Id = id, CompanyId = companyId };
            _warehouseRepoMock.Setup(r => r.GetById(id)).ReturnsAsync(model);

            await _warehouseService.Delete(id, companyId);

            _userServiceMock.Verify(u => u.RemoveWarehouse(id, companyId), Times.Once);
            _inventoryServiceMock.Verify(i => i.DeleteByWarehouseId(id), Times.Once);
            _warehouseRepoMock.Verify(r => r.Delete(id), Times.Once);
        }

        [Fact]
        public async Task GetByIds_ShouldThrowKeyNotFound_WhenSomeIdsMissing()
        {
            var ids = new[] { "1", "2" };
            _warehouseRepoMock.Setup(r => r.GetByIds(ids))
                .ReturnsAsync(new List<WarehouseModel> { new WarehouseModel() }); 

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _warehouseService.GetByIds(ids, "c1"));
        }

        [Fact]
        public async Task Update_ShouldThrowArgumentException_WhenValidationFails()
        {
            var id = "w1";
            var companyId = "c1";
            var model = new WarehouseModel { Id = id, CompanyId = companyId, Name = "Old" };
            var update = new WarehouseUpdate { Name = "" }; 


            var invalidDto = new WarehouseDTO { Name = "" };

            _warehouseRepoMock.Setup(r => r.GetById(id)).ReturnsAsync(model);
            _mapperMock.Setup(m => m.Map<WarehouseDTO>(It.IsAny<WarehouseModel>())).Returns(invalidDto);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _warehouseService.Update(id, update, companyId));
        }

        [Fact]
        public async Task Update_ShouldSucceed_WhenDataIsCorrect()
        {
            var id = "w1";
            var companyId = "c1";
            var model = new WarehouseModel { Id = id, CompanyId = companyId, Name = "Old" };
            var update = new WarehouseUpdate { Name = "New Name", Area = 500 };
            var validDto = new WarehouseDTO { Name = "New Name", Area = 500 };

            _warehouseRepoMock.Setup(r => r.GetById(id)).ReturnsAsync(model);
            _mapperMock.Setup(m => m.Map<WarehouseDTO>(It.IsAny<WarehouseModel>())).Returns(validDto);
            _warehouseRepoMock.Setup(r => r.Update(model)).ReturnsAsync(model);
            _mapperMock.Setup(m => m.Map<WarehouseResponse>(model)).Returns(new WarehouseResponse { Name = "New Name" });

            var result = await _warehouseService.Update(id, update, companyId);

            Assert.Equal("New Name", model.Name);
            Assert.Equal(500, model.Area);
            _warehouseRepoMock.Verify(r => r.Update(model), Times.Once);
        }

        [Theory]
        [InlineData(-1, 10)] 
        [InlineData(1, -5)]  
        public async Task Get_ShouldThrowArgumentException_WhenPaginationInvalid(int page, int size)
        {
            var query = new WarehouseQuery { Page = page, PageSize = size };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _warehouseService.Get("c1", query));
        }

        [Fact]
        public async Task Get_ShouldThrowArgumentException_WhenAreaRangeInvalid()
        {
            var query = new WarehouseQuery { MinArea = 100, MaxArea = 50 }; 

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _warehouseService.Get("c1", query));
        }
    }
}