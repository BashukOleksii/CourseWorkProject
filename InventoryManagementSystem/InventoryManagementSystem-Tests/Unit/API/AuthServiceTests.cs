using Moq;
using AutoMapper;
using InventorySystem_API.User.Services;
using InventorySystem_API.User.Repositories;
using InventorySystem_API.User.Model;
using InventorySystem_API.Warehouse.Service;
using InventorySystem_API.Service.Image;
using InventorySystem_Shared.User;
using Microsoft.Extensions.Options;

namespace InventoryManagementSystem_Tests.Unit.API
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHasher> _hasherMock;
        private readonly Mock<IWarehouseService> _warehouseServiceMock;
        private readonly Mock<IImageService> _imageServiceMock;
        private readonly TokenGenerator _tokenGenerator;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _hasherMock = new Mock<IHasher>();
            _warehouseServiceMock = new Mock<IWarehouseService>();
            _imageServiceMock = new Mock<IImageService>();

            var jwtOptions = Options.Create(new JWTSettingOptions
            {
                SecretKey = "super_secret_key_12345678901234567890",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiryInMinutes = 60,
                RefreshTokenExpiryInDays = 7
            });

            _tokenGenerator = new TokenGenerator(jwtOptions);

            _authService = new AuthService(
                _userRepoMock.Object,
                _tokenGenerator,
                _hasherMock.Object,
                _mapperMock.Object,
                jwtOptions,
                _warehouseServiceMock.Object,
                _imageServiceMock.Object
            );
        }

        [Fact]
        public async Task LogIn_ShouldThrowArgumentException_WhenUserNotFound()
        {
            _userRepoMock.Setup(r => r.GetByEmail(It.IsAny<string>())).ReturnsAsync((UserModel?)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _authService.LogIn(new UserLogin { Email = "test@test.com", Password = "123" }));
        }

        [Fact]
        public async Task LogIn_ShouldThrowArgumentException_WhenPasswordInvalid()
        {
            var user = new UserModel { Email = "test@test.com", PasswordHash = "hashed" };
            _userRepoMock.Setup(r => r.GetByEmail(user.Email)).ReturnsAsync(user);
            _hasherMock.Setup(h => h.Verify("wrong", "hashed")).Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _authService.LogIn(new UserLogin { Email = user.Email, Password = "wrong" }));
        }

        [Fact]
        public async Task LogIn_ShouldReturnTokens_WhenCredentialsCorrect()
        {
            var user = new UserModel { Id = "u1", Email = "test@test.com", PasswordHash = "hashed", CompanyId = "c1" };
            _userRepoMock.Setup(r => r.GetByEmail(user.Email)).ReturnsAsync(user);
            _hasherMock.Setup(h => h.Verify("correct", "hashed")).Returns(true);

            var result = await _authService.LogIn(new UserLogin { Email = user.Email, Password = "correct" });

            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            _userRepoMock.Verify(r => r.Update(It.IsAny<UserModel>()), Times.Once); 
        }

        [Fact]
        public async Task Register_ShouldThrowArgumentException_WhenEmailTaken()
        {
            _userRepoMock.Setup(r => r.GetByEmail("exists@test.com")).ReturnsAsync(new UserModel());

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _authService.Register(new UserRegister { Email = "exists@test.com" }, null));
        }

        [Fact]
        public async Task Register_ShouldAssignAllWarehouses_WhenUserIsAdmin()
        {
            var registerDto = new UserRegister { Email = "admin@test.com", UserRole = UserRole.admin, CompanyId = "c1" };
            var userModel = new UserModel { Email = registerDto.Email, UserRole = UserRole.admin, CompanyId = "c1" };
            var warehouseIds = new List<string> { "w1", "w2" };

            _userRepoMock.Setup(r => r.GetByEmail(registerDto.Email)).ReturnsAsync((UserModel?)null);
            _mapperMock.Setup(m => m.Map<UserModel>(registerDto)).Returns(userModel);
            _warehouseServiceMock.Setup(w => w.GetIdsByCompanyId("c1")).ReturnsAsync(warehouseIds);
            _imageServiceMock.Setup(i => i.GetDefaultImage("User")).Returns("default.png");

            await _authService.Register(registerDto, null);

            Assert.Equal(warehouseIds, userModel.WarehouseIds);
            _userRepoMock.Verify(r => r.Create(userModel), Times.Once);
        }

        [Fact]
        public async Task Refresh_ShouldThrowArgumentException_WhenTokenInvalid()
        {
            var user = new UserModel
            {
                Id = "u1",
                RefeshToken = new RefeshToken { RefreshToken = "valid_token", ExpiryDate = DateTime.UtcNow.AddDays(1) }
            };
            _userRepoMock.Setup(r => r.GetById("u1")).ReturnsAsync(user);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _authService.Refresh(new UserRefresh { UserId = "u1", RefreshToken = "WRONG_TOKEN" }));
        }

        [Fact]
        public async Task Refresh_ShouldThrowArgumentException_WhenTokenExpired()
        {
            var user = new UserModel
            {
                Id = "u1",
                RefeshToken = new RefeshToken { RefreshToken = "token", ExpiryDate = DateTime.UtcNow.AddDays(-1) } 
            };
            _userRepoMock.Setup(r => r.GetById("u1")).ReturnsAsync(user);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _authService.Refresh(new UserRefresh { UserId = "u1", RefreshToken = "token" }));
        }
    }
}