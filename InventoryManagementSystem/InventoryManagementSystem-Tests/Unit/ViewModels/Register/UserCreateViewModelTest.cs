using Moq;
using Xunit;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.User;
using System.Threading.Tasks;

namespace InventoryManagementSystem_Tests.Unit.ViewModels.Register
{
    public class UserCreateViewModelTest
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly UserCreateViewModel _viewModel;

        public UserCreateViewModelTest()
        {
            _authServiceMock = new Mock<IAuthService>();
            _viewModel = new UserCreateViewModel(_authServiceMock.Object);
        }

        [Theory]
        [InlineData("", "test@mail.com", "password")]
        [InlineData("Name", "", "password")]
        [InlineData("Name", "test@mail.com", "")]
        public async Task Register_ShouldShowAlert_WhenRequiredFieldsAreMissing(string name, string email, string password)
        {
            _viewModel.Name = name;
            _viewModel.Email = email;
            _viewModel.Password = password;

            await _viewModel.RegisterCommand.ExecuteAsync(null);

            _authServiceMock.Verify(s => s.Register(It.IsAny<UserRegister>(), It.IsAny<FileResult>()), Times.Never);
        }

        [Fact]
        public async Task Register_ShouldCallAuthService_WithAdminRole()
        {
            _viewModel.CompanyId = "comp-123";
            _viewModel.Name = "Адміністратор";
            _viewModel.Email = "admin@test.com";
            _viewModel.Password = "secure123";

            await _viewModel.RegisterCommand.ExecuteAsync(null);

            _authServiceMock.Verify(s => s.Register(It.Is<UserRegister>(u =>
                u.CompanyId == "comp-123" &&
                u.UserRole == UserRole.admin &&
                u.Email == "admin@test.com"),
                null), 
                Times.Once);
        }

    }
}