using Moq;
using Xunit;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using System.Threading.Tasks;

namespace InventoryManagementSystem_Tests.Unit.ViewModels.Login
{
    public class ForgotPasswordViewModelTest
    {
        private readonly Mock<IResetPasswordService> _resetServiceMock;
        private readonly ForgotPasswordViewModel _viewModel;

        public ForgotPasswordViewModelTest()
        {
            _resetServiceMock = new Mock<IResetPasswordService>();
            _viewModel = new ForgotPasswordViewModel(_resetServiceMock.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            Assert.False(_viewModel.IsCodeSent);
            Assert.Null(_viewModel.Email);
        }

        [Fact]
        public async Task SendCode_ShouldSetIsCodeSentToTrue_WhenSuccessful()
        {
            _viewModel.Email = "test@example.com";

            _resetServiceMock
                .Setup(s => s.RequestResetCode(_viewModel.Email))
                .Returns(Task.CompletedTask);

            await _viewModel.SendCodeCommand.ExecuteAsync(null);

            Assert.True(_viewModel.IsCodeSent);
            _resetServiceMock.Verify(s => s.RequestResetCode(_viewModel.Email), Times.Once);
        }

        [Fact]
        public async Task SendCode_ShouldNotProceed_IfEmailIsEmpty()
        {
            _viewModel.Email = "";

            await _viewModel.SendCodeCommand.ExecuteAsync(null);

            Assert.False(_viewModel.IsCodeSent);
            _resetServiceMock.Verify(s => s.RequestResetCode(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ResetPassword_ShouldCallService_WithCorrectData()
        {
            _viewModel.Email = "test@example.com";
            _viewModel.ResetCode = "123456";
            _viewModel.NewPassword = "NewSecurePassword123";

            _resetServiceMock
                .Setup(s => s.ConfirmResetPassword(_viewModel.Email, _viewModel.ResetCode, _viewModel.NewPassword))
                .Returns(Task.CompletedTask);

            await _viewModel.ResetPasswordCommand.ExecuteAsync(null);

            _resetServiceMock.Verify(s => s.ConfirmResetPassword(
                _viewModel.Email,
                _viewModel.ResetCode,
                _viewModel.NewPassword), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_ShouldNotCallService_IfFieldsAreMissing()
        {
            _viewModel.ResetCode = ""; 
            _viewModel.NewPassword = "password";

            await _viewModel.ResetPasswordCommand.ExecuteAsync(null);

            _resetServiceMock.Verify(s => s.ConfirmResetPassword(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RunBusyTask_ShouldKeepIsCodeSentFalse_IfRequestFails()
        {
            _viewModel.Email = "fail@test.com";
            _resetServiceMock
                .Setup(s => s.RequestResetCode(It.IsAny<string>()))
                .ThrowsAsync(new System.Net.Http.HttpRequestException());

            await _viewModel.SendCodeCommand.ExecuteAsync(null);

            Assert.False(_viewModel.IsCodeSent); 
            Assert.False(_viewModel.IsBusy);     
        }
    }
}