using Moq;
using Xunit;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Inventory.Manufacturer;
using System.Threading.Tasks;

namespace InventoryManagementSystem_Tests.Unit.ViewModels.Manufacturer
{
    public class ManufacturerDetailsViewModelTest
    {
        private readonly Mock<IManufacturerService> _serviceMock;
        private readonly ManufacturerDetailsViewModel _viewModel;

        public ManufacturerDetailsViewModelTest()
        {
            _serviceMock = new Mock<IManufacturerService>();
            _viewModel = new ManufacturerDetailsViewModel(_serviceMock.Object);
        }

        [Fact]
        public void Constructor_ShouldSetDefaultTitle()
        {
            Assert.Equal("Новий виробник", _viewModel.Title);
            Assert.Null(_viewModel.Manufacturer);
        }

        [Fact]
        public void OnManufacturerChanged_ShouldUpdateFieldsAndTitle()
        {
            var existingManufacturer = new InventoryManufacturer
            {
                Name = "Intel",
                Country = "USA"
            };

            _viewModel.Manufacturer = existingManufacturer;

            Assert.Equal("Intel", _viewModel.Name);
            Assert.Equal("USA", _viewModel.Country);
            Assert.Equal("Редагування", _viewModel.Title);
        }

        [Theory]
        [InlineData("", "Ukraine")]
        [InlineData("Ajax", "")]
        [InlineData(" ", " ")]
        public async Task Save_ShouldNotCallService_WhenFieldsAreInvalid(string name, string country)
        {
            _viewModel.Name = name;
            _viewModel.Country = country;

            await _viewModel.SaveCommand.ExecuteAsync(null);

            _serviceMock.Verify(s => s.UpsertManufacturerAsync(
                It.IsAny<InventoryManufacturer>(),
                It.IsAny<string>()), Times.Never);
        }

    }
}