using Moq;
using Xunit;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.AddressClass;
using InventorySystem_Shared.Company;
using System.Threading.Tasks;

namespace InventoryManagementSystem_Tests.Unit.ViewModels.Company
{
    public class CompanyCreateViewModelTests
    {
        private readonly Mock<ICompanyService> _companyServiceMock;
        private readonly CompanyCreateViewModel _viewModel;

        public CompanyCreateViewModelTests()
        {
            _companyServiceMock = new Mock<ICompanyService>();
            _viewModel = new CompanyCreateViewModel(_companyServiceMock.Object);
        }

        [Fact]
        public async Task SaveCompany_ShouldCallService_WhenDataIsValid()
        {
            _viewModel.Name = "Test";
            _viewModel.Description = "Desc";
            _viewModel.Phone = "123";
            _viewModel.SelectedAddress = new Address { City = "Km", Street = "Central", HouseNumber = "1" };

            _companyServiceMock
                .Setup(s => s.CreateCompany(It.IsAny<CompanyDTO>()))
                .ReturnsAsync("123");

            await _viewModel.SaveCompanyCommand.ExecuteAsync(null);

            _companyServiceMock.Verify(s => s.CreateCompany(It.IsAny<CompanyDTO>()), Times.Once);
        }

        [Fact]
        public async Task SaveCompany_ShouldNotCallService_WhenFieldsAreEmpty()
        {
            _viewModel.Name = ""; 

            await _viewModel.SaveCompanyCommand.ExecuteAsync(null);

            _companyServiceMock.Verify(s => s.CreateCompany(It.IsAny<CompanyDTO>()), Times.Never);
        }
    }
}