using Moq;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Inventory;
using InventorySystem_Shared.Inventory.Manufacturer;

namespace InventoryManagementSystem_Tests.Unit.ViewModels
{
    public class InventoryDetailsViewModelTests
    {
        private readonly Mock<IInventoryService> _inventoryServiceMock;
        private readonly Mock<IManufacturerService> _manufacturerServiceMock;
        private readonly InventoryDetailsViewModel _viewModel;

        public InventoryDetailsViewModelTests()
        {
            _inventoryServiceMock = new Mock<IInventoryService>();
            _manufacturerServiceMock = new Mock<IManufacturerService>();

            var manufacturers = new List<InventoryManufacturer>
            {
                new() { Name = "Samsung", Country = "South Korea" },
                new() { Name = "LG", Country = "USA" }
            };
            _manufacturerServiceMock.Setup(s => s.GetManufacturersAsync()).ReturnsAsync(manufacturers);

            _viewModel = new InventoryDetailsViewModel(_inventoryServiceMock.Object, _manufacturerServiceMock.Object);
        }

        [Fact]
        public async Task Initialize_ShouldLoadManufacturers_AndSetTitleForNewItem()
        {
            _viewModel.ItemId = string.Empty;

            await _viewModel.InitializeCommand.ExecuteAsync(null);

            Assert.Equal(2, _viewModel.Manufacturers.Count);
            Assert.Equal("Новий товар", _viewModel.Title);
            _inventoryServiceMock.Verify(s => s.GetById(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Initialize_ShouldLoadItemData_AndSetSelectedManufacturer()
        {
            _viewModel.ItemId = "item-123";
            var mockItem = new InventoryResponse
            {
                Id = "item-123",
                Name = "Friger 15",
                Manufacturer = new InventoryManufacturer { Name = "LG" },
                CustomFileds = new Dictionary<string, string> { { "Color", "Black" } }
            };

            _inventoryServiceMock.Setup(s => s.GetById("item-123")).ReturnsAsync(mockItem);

            await _viewModel.InitializeCommand.ExecuteAsync(null);

            Assert.Equal("Редагування", _viewModel.Title);
            Assert.Equal("Friger 15", _viewModel.Name);
            Assert.Equal("LG", _viewModel.SelectedManufacturer?.Name);
            Assert.Single(_viewModel.DynamicFields);
            Assert.Equal("Color", _viewModel.DynamicFields[0].Key);
        }

        [Fact]
        public void DynamicFields_AddAndRemove_ShouldUpdateCollection()
        {
            _viewModel.AddFieldCommand.Execute(null);
            _viewModel.AddFieldCommand.Execute(null);
            var fieldToRemove = _viewModel.DynamicFields.First();
            _viewModel.RemoveFieldCommand.Execute(fieldToRemove);

            Assert.Single(_viewModel.DynamicFields);
        }

        [Fact]
        public async Task Save_ShouldNotCallService_IfNameIsMissing()
        {
            _viewModel.Name = "";
            _viewModel.SelectedManufacturer = new InventoryManufacturer();

            await _viewModel.SaveCommand.ExecuteAsync(null);

            _inventoryServiceMock.Verify(s => s.CreateItem(It.IsAny<string>(), It.IsAny<InventoryCreate>(), It.IsAny<FileResult>()), Times.Never);
        }

        [Fact]
        public async Task Save_ShouldCallCreate_WhenItemIdIsEmpty()
        {
            _viewModel.WarehouseId = "wh-1";
            _viewModel.ItemId = string.Empty;
            _viewModel.Name = "Test Item";
            _viewModel.SelectedManufacturer = new InventoryManufacturer { Name = "Samsung" };
            _viewModel.DynamicFields.Add(new CustomFieldItem { Key = "Capacity", Value = "100" });

            await _viewModel.SaveCommand.ExecuteAsync(null);

            _inventoryServiceMock.Verify(s => s.CreateItem(
                "wh-1",
                It.Is<InventoryCreate>(c => c.Name == "Test Item" && c.CustomFileds.ContainsKey("Capacity")),
                It.IsAny<FileResult>()),
                Times.Once);
        }

        [Fact]
        public async Task Save_ShouldCallUpdate_WhenItemIdIsPresent()
        {
            _viewModel.ItemId = "existing-id";
            _viewModel.Name = "Updated Name";
            _viewModel.SelectedManufacturer = new InventoryManufacturer { Name = "LG", Country = "USA" };

            await _viewModel.SaveCommand.ExecuteAsync(null);

            _inventoryServiceMock.Verify(s => s.UpdateItem(
                "existing-id",
                It.Is<InventoryUpdate>(u => u.Name == "Updated Name"),
                It.IsAny<FileResult>()),
                Times.Once);
        }
    }
}