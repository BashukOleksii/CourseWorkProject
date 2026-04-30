using Moq;
using Xunit;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.Helper;
using InventorySystem_Shared.User;

namespace InventoryManagementSystem_Tests.Unit.ViewModels.Login
{
    public class LoginViewModelTest
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IUserContextService> _userContextMock;
        private readonly LoginViewModel _viewModel;

        public LoginViewModelTest()
        {
            _authServiceMock = new Mock<IAuthService>();
            _userContextMock = new Mock<IUserContextService>();

            _viewModel = new LoginViewModel(_authServiceMock.Object, _userContextMock.Object);
        }

        [Theory]
        [InlineData("", "password")]
        [InlineData("email@test.com", "")]
        [InlineData(" ", " ")]
        public async Task Login_ShouldNotCallAuthService_WhenCredentialsAreInvalid(string email, string password)
        {
            _viewModel.Email = email;
            _viewModel.Password = password;

            await _viewModel.LoginCommand.ExecuteAsync(null);

            _authServiceMock.Verify(s => s.Login(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Login_ShouldCallLoginAndCheckRole_WhenDataIsValid()
        {
            _viewModel.Email = "admin@system.com";
            _viewModel.Password = "securePassword";

            _userContextMock.SetupGet(u => u.CurrentUser)
                           .Returns(new UserResponse { UserRole = UserRole.manager });

            await _viewModel.LoginCommand.ExecuteAsync(null);

            _authServiceMock.Verify(s => s.Login(_viewModel.Email, _viewModel.Password), Times.Once);
            _userContextMock.VerifyGet(u => u.CurrentUser, Times.AtLeastOnce);
        }

        [Fact]
        public async Task Login_ShouldStayOnPage_IfAuthThrowsException()
        {
            _viewModel.Email = "test@test.com";
            _viewModel.Password = "123";

            _authServiceMock.Setup(s => s.Login(It.IsAny<string>(), It.IsAny<string>()))
                            .ThrowsAsync(new System.Net.Http.HttpRequestException());

            await _viewModel.LoginCommand.ExecuteAsync(null);

            Assert.False(_viewModel.IsBusy); 
            _userContextMock.VerifyGet(u => u.CurrentUser, Times.Never); 
        }

    }
}