using Moq;
using Xunit;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Company;
using InventorySystem_Shared.AddressClass;

namespace InventoryManagementSystem_Tests
{
    public class CompanyViewModelTests
    {
        private readonly Mock<ICompanyService> _serviceMock;
        private readonly CompanyResponse _mockCompany;

        public CompanyViewModelTests()
        {
            _serviceMock = new Mock<ICompanyService>();

            _mockCompany = new CompanyResponse
            {
                Name = "Епіцентр",
                Description = "Будівельний гіпермаркет",
                Phone = "0800123456",
                Address = new Address { City = "Хмельницький", Street = "Зарічанська" }
            };

            _serviceMock.Setup(s => s.GetMyCompany()).ReturnsAsync(_mockCompany);
        }

        [Fact]
        public async Task Constructor_ShouldLoadCompanyData()
        {
            var viewModel = new CompanyViewModel(_serviceMock.Object);

            await viewModel.LoadCompany();

            Assert.Equal(_mockCompany.Name, viewModel.Name);
            Assert.Equal(_mockCompany.Description, viewModel.Description);
            Assert.Equal(_mockCompany.Address?.City, viewModel.Address?.City);

            _serviceMock.Verify(s => s.GetMyCompany(), Times.Once);
        }

        [Fact]
        public async Task SaveChanges_ShouldSendUpdatedDataToService()
        {
            var viewModel = new CompanyViewModel(_serviceMock.Object);
            await viewModel.LoadCompany(); 

            string newName = "Нова назва";
            viewModel.Name = newName;

            await viewModel.SaveChangesCommand.ExecuteAsync(null);

            _serviceMock.Verify(s => s.UpdateMyCompany(It.Is<CompanyUpdate>(u =>
                u.Name == newName &&
                u.Phone == _mockCompany.Phone)), Times.Once);
        }

        [Fact]
        public async Task RunBusyTask_ShouldHandleException_AndResetIsBusy()
        {
            _serviceMock.Setup(s => s.UpdateMyCompany(It.IsAny<CompanyUpdate>()))
                        .ThrowsAsync(new HttpRequestException());

            var viewModel = new CompanyViewModel(_serviceMock.Object);
            await Task.Delay(100);

            await viewModel.SaveChangesCommand.ExecuteAsync(null);

            Assert.False(viewModel.IsBusy);
        }
    }
}