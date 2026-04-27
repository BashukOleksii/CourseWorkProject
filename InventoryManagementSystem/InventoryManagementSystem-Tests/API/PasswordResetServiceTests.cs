using Moq;
using InventorySystem_API.User.Services;
using InventorySystem_API.User.Repositories;
using InventorySystem_API.User.Models;
using InventorySystem_API.User.Model;
using Microsoft.Extensions.Options;

namespace InventoryManagementSystem_Tests
{
    public class PasswordResetServiceTests
    {
        private readonly Mock<IPasswordResetRepository> _resetRepoMock;
        private readonly Mock<IHasher> _hasherMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly PasswordResetService _passwordResetService;

        public PasswordResetServiceTests()
        {
            _resetRepoMock = new Mock<IPasswordResetRepository>();
            _hasherMock = new Mock<IHasher>();
            _userRepoMock = new Mock<IUserRepository>();

            var emailOptions = Options.Create(new EmailSettingOptions
            {
                From = "test@gmail.com",
                AppPassword = "password"
            });

            _passwordResetService = new PasswordResetService(
                _resetRepoMock.Object,
                _hasherMock.Object,
                _userRepoMock.Object,
                emailOptions
            );
        }

        [Fact]
        public async Task ChangePassword_ShouldThrowArgumentException_WhenPasswordIsWeak()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _passwordResetService.ChangePassword("test@test.com", "weakpass"));
        }

        [Fact]
        public async Task ChangePassword_ShouldUpdateUserAndCleanup_WhenValid()
        {
            var email = "user@test.com";
            var pass = "ValidPassword123!";
            var user = new UserModel { Email = email };

            _userRepoMock.Setup(r => r.GetByEmail(email)).ReturnsAsync(user);
            _hasherMock.Setup(h => h.Hash(pass)).Returns("new_hash");

            await _passwordResetService.ChangePassword(email, pass);

            Assert.Equal("new_hash", user.PasswordHash);
            _userRepoMock.Verify(r => r.Update(user), Times.Once);
            _resetRepoMock.Verify(r => r.Delete(email), Times.Once); 
        }

        [Fact]
        public async Task CheckCode_ShouldThrowArgumentException_WhenRecordNotFound()
        {
            _resetRepoMock.Setup(r => r.Get("test@test.com")).ReturnsAsync((ResetPasswordModel?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _passwordResetService.CheckCode("test@test.com", "1234567"));
            Assert.Contains("Час вийшов", ex.Message);
        }

        [Fact]
        public async Task CheckCode_ShouldThrowArgumentException_WhenCodeIsIncorrect()
        {
            var email = "test@test.com";
            var resetModel = new ResetPasswordModel { CodeHash = "stored_hash" };
            _resetRepoMock.Setup(r => r.Get(email)).ReturnsAsync(resetModel);
            _hasherMock.Setup(h => h.Verify("1111111", "stored_hash")).Returns(false);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _passwordResetService.CheckCode(email, "1111111"));
            Assert.Contains("Невірний код", ex.Message);
        }

        [Fact]
        public async Task CheckCode_ShouldPass_WhenCodeIsCorrect()
        {
            var email = "test@test.com";
            var resetModel = new ResetPasswordModel { CodeHash = "stored_hash" };
            _resetRepoMock.Setup(r => r.Get(email)).ReturnsAsync(resetModel);
            _hasherMock.Setup(h => h.Verify("7654321", "stored_hash")).Returns(true);

            await _passwordResetService.CheckCode(email, "7654321");

            _hasherMock.Verify(h => h.Verify("7654321", "stored_hash"), Times.Once);
        }

        [Fact]
        public async Task GenerateResetCode_ShouldThrowArgumentException_WhenUserNotFound()
        {
            _userRepoMock.Setup(r => r.GetByEmail(It.IsAny<string>())).ReturnsAsync((UserModel?)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _passwordResetService.GenerateResetCode("unknown@test.com"));
        }
    }
}