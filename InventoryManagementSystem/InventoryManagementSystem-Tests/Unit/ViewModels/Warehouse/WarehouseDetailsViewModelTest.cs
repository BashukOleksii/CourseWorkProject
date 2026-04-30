using Moq;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Warehouse;
using InventorySystem_Shared.AddressClass;

namespace InventoryManagementSystem_Tests.Unit.ViewModels.Warehouse
{
    public class WarehouseDetailsViewModelTests
    {
        private readonly Mock<IWarehouseService> _warehouseServiceMock;
        private readonly WarehouseDetailsViewModel _viewModel;

        public WarehouseDetailsViewModelTests()
        {
            _warehouseServiceMock = new Mock<IWarehouseService>();
            _viewModel = new WarehouseDetailsViewModel(_warehouseServiceMock.Object);
        }

        [Fact]
        public void InitialState_ShouldHaveDefaultValues()
        {
            Assert.Equal("Новий склад", _viewModel.Title);
            Assert.False(_viewModel.HasId);
            Assert.False(_viewModel.HasAddress);
            Assert.Equal("Адресу не обрано", _viewModel.AddressDisplay);
        }

        [Fact]
        public void OnSelectedAddressChanged_ShouldUpdateAddressDisplay()
        {
            var address = new Address
            {
                City = "Хмельницький",
                Street = "Проскурівська",
                HouseNumber = "1"
            };

            _viewModel.SelectedAddress = address;

            Assert.True(_viewModel.HasAddress);
            Assert.Equal("Хмельницький, Проскурівська 1", _viewModel.AddressDisplay);
        }

        [Fact]
        public async Task OnWarehouseIdChanged_ShouldLoadWarehouseData()
        {
            var warehouseId = "wh-123";
            var mockWarehouse = new WarehouseResponse
            {
                Name = "Головний склад",
                Description = "Опис",
                Area = 500.5,
                Address = new Address { City = "Київ" }
            };

            _warehouseServiceMock
                .Setup(s => s.GetWarehouseById(warehouseId))
                .ReturnsAsync(mockWarehouse);

            _viewModel.WarehouseId = warehouseId;

            await Task.Delay(50);

            Assert.Equal("Редагування складу", _viewModel.Title);
            Assert.Equal("Головний склад", _viewModel.Name);
            Assert.Equal(500.5, _viewModel.Area);
            Assert.True(_viewModel.HasId);
            _warehouseServiceMock.Verify(s => s.GetWarehouseById(warehouseId), Times.Once);
        }

        [Fact]
        public async Task Save_ShouldCallCreate_WhenIdIsEmpty()
        {
            _viewModel.WarehouseId = string.Empty;
            _viewModel.Name = "Новий склад";
            _viewModel.Area = 100;
            _viewModel.SelectedAddress = new Address { City = "Львів" };

            await _viewModel.SaveCommand.ExecuteAsync(null);

            _warehouseServiceMock.Verify(s => s.CreateWarehouse(
                It.Is<WarehouseDTO>(d => d.Name == "Новий склад" && d.Area == 100)),
                Times.Once);
        }

        [Fact]
        public async Task Save_ShouldCallUpdate_WhenIdIsPresent()
        {
            _viewModel.WarehouseId = "existing-id";
            _viewModel.Name = "Оновлена назва";

            await _viewModel.SaveCommand.ExecuteAsync(null);

            _warehouseServiceMock.Verify(s => s.UpdateWarehouse(
                "existing-id",
                It.Is<WarehouseUpdate>(u => u.Name == "Оновлена назва")),
                Times.Once);
        }

    
    }
}