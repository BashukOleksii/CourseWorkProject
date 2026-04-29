using Moq;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.Helper;
using InventorySystem_Shared.User;
using InventorySystem_Shared.Company;

namespace InventoryManagementSystem_Tests.Unit.ViewModels.Login
{
    public class ProfileViewModelTest
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ICompanyService> _companyServiceMock;
        private readonly Mock<IUserContextService> _userContextMock;
        private readonly ProfileViewModel _viewModel;

        public ProfileViewModelTest()
        {
            _userServiceMock = new Mock<IUserService>();
            _companyServiceMock = new Mock<ICompanyService>();
            _userContextMock = new Mock<IUserContextService>();

            var mockUser = new UserResponse
            {
                Id = "user-12345",
                Name = "Олексій",
                Email = "test@mail.com",
                UserRole = UserRole.manager,
                PhotoURI = "images/avatar.png"
            };
            _userContextMock.SetupGet(x => x.CurrentUser).Returns(mockUser);

            _viewModel = new ProfileViewModel(
                _userServiceMock.Object,
                _companyServiceMock.Object,
                _userContextMock.Object);
        }

        [Fact]
        public async Task LoadData_ShouldFillProperties_FromContextAndService()
        {
            var mockCompany = new CompanyResponse { Name = "Моя Компанія" };
            _companyServiceMock.Setup(s => s.GetMyCompany()).ReturnsAsync(mockCompany);

            await _viewModel.LoadData();

            Assert.Equal("Олексій", _viewModel.Name);
            Assert.Equal("test@mail.com", _viewModel.Email);
            Assert.Equal("Моя Компанія", _viewModel.Company.Name);
            _companyServiceMock.Verify(s => s.GetMyCompany(), Times.Once);
        }

        [Fact]
        public async Task SaveChanges_ShouldFail_WhenPasswordsDoNotMatch()
        {
            _viewModel.NewPassword = "123";
            _viewModel.ConfirmPassword = "456";

            await _viewModel.SaveChangesCommand.ExecuteAsync(null);

            _userServiceMock.Verify(s => s.UpdateUser(It.IsAny<string>(), It.IsAny<UserUpdate>()), Times.Never);
        }

        [Fact]
        public async Task SaveChanges_ShouldUpdateProperties_OnSuccess()
        {
            _viewModel.EditName = "Нове Ім'я";
            _viewModel.EditEmail = "new@mail.com";
            _viewModel.IsEditMode = true;

            var updatedUser = new UserResponse
            {
                Name = "Нове Ім'я",
                Email = "new@mail.com",
                PhotoURI = "new_photo.png"
            };

            _userServiceMock
                .Setup(s => s.UpdateUser(It.IsAny<string>(), It.IsAny<UserUpdate>()))
                .ReturnsAsync(updatedUser);

            await _viewModel.SaveChangesCommand.ExecuteAsync(null);

            Assert.Equal("Нове Ім'я", _viewModel.Name);
            Assert.False(_viewModel.IsEditMode);
            Assert.Empty(_viewModel.NewPassword);
            _userServiceMock.Verify(s => s.UpdateUser("user-12345", It.Is<UserUpdate>(u => u.Name == "Нове Ім'я")), Times.Once);
        }

        
    }
}