using Moq;
using Xunit;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.AddressClass;

namespace InventoryManagementSystem_Tests.Unit.ViewModels
{
    public class AddressCreateViewModelTest
    {
        private readonly Mock<IAddressService> _addressServiceMock;
        private readonly AddressCreateViewModel _viewModel;

        public AddressCreateViewModelTest()
        {
            _addressServiceMock = new Mock<IAddressService>();
            _viewModel = new AddressCreateViewModel(_addressServiceMock.Object);
        }

        [Fact]
        public void SettingInitialAddress_ShouldUpdateAllFields()
        {
            var initial = new Address
            {
                City = "Хмельницький",
                Street = "Зарічанська",
                Latitude = 49.42,
                Longitude = 26.98
            };

            _viewModel.InitialAddress = initial;

            Assert.Equal("Хмельницький", _viewModel.City);
            Assert.Equal("Зарічанська", _viewModel.Street);
            Assert.Equal(49.42, _viewModel.Latitude);
        }

        [Fact]
        public async Task GetByCoords_ShouldUpdateFields_WhenSuccessful()
        {
            _viewModel.Latitude = 50.45;
            _viewModel.Longitude = 30.52;

            var expectedResult = new Address { City = "Київ", Street = "Хрещатик" };

            _addressServiceMock
                .Setup(s => s.GetByAddress(It.IsAny<Address>(), "location"))
                .ReturnsAsync(expectedResult);

            await _viewModel.GetByCoordsCommand.ExecuteAsync(null);

            Assert.Equal("Київ", _viewModel.City);
            Assert.Equal("Хрещатик", _viewModel.Street);
            _addressServiceMock.Verify(s => s.GetByAddress(It.Is<Address>(a =>
                a.Latitude == 50.45 && a.Longitude == 30.52), "location"), Times.Once);
        }

        [Fact]
        public async Task GetByCoords_ShouldShowError_WhenCoordsAreNull()
        {
            _viewModel.Latitude = null;
            await _viewModel.GetByCoordsCommand.ExecuteAsync(null);
            _addressServiceMock.Verify(s => s.GetByAddress(It.IsAny<Address>(), It.IsAny<string>()), Times.Never);
        }


        [Fact]
        public async Task GetByText_ShouldCallService_WithCorrectFields()
        {
            _viewModel.City = "Львів";
            _viewModel.Street = "Городоцька";

            _addressServiceMock
                .Setup(s => s.GetByAddress(It.IsAny<Address>(), "address"))
                .ReturnsAsync(new Address { Latitude = 49.83, Longitude = 24.01 });

            await _viewModel.GetByTextCommand.ExecuteAsync(null);

            Assert.Equal(49.83, _viewModel.Latitude);
            _addressServiceMock.Verify(s => s.GetByAddress(It.Is<Address>(a =>
                a.City == "Львів" && a.Street == "Городоцька"), "address"), Times.Once);
        }
    }
}