using Moq;
using AutoMapper;
using InventorySystem_API.User.Services;
using InventorySystem_API.User.Repositories;
using InventorySystem_API.User.Model;
using InventorySystem_API.User.Validator;
using InventorySystem_API.Service.Image;
using InventorySystem_Shared.User;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace InventoryManagementSystem_Tests.Unit.API
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHasher> _hasherMock;
        private readonly Mock<IImageService> _imageServiceMock;
        private readonly UserModelValidator _validator;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _hasherMock = new Mock<IHasher>();
            _imageServiceMock = new Mock<IImageService>();
            _validator = new UserModelValidator();

            _userService = new UserService(
                _userRepoMock.Object,
                _mapperMock.Object,
                _validator,
                _hasherMock.Object,
                _imageServiceMock.Object
            );
        }

        [Fact]
        public async Task GetById_ShouldThrowKeyNotFound_WhenUserDoesNotExist()
        {
            _userRepoMock.Setup(r => r.GetById("u1")).ReturnsAsync((UserModel?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                ((IUserService)_userService).GetById("u1", "c1"));
        }

        [Fact]
        public async Task GetById_ShouldThrowArgumentException_WhenCompanyIdMismatch()
        {
            var user = new UserModel { Id = "u1", CompanyId = "c1" };
            _userRepoMock.Setup(r => r.GetById("u1")).ReturnsAsync(user);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                ((IUserService)_userService).GetById("u1", "OTHER_COMPANY"));
        }

        [Fact]
        public async Task UpdateWarehouses_ShouldUpdateList_WhenUserFound()
        {
            var user = new UserModel { Id = "u1", CompanyId = "c1", WarehouseIds = new List<string>() };
            var newWarehouses = new[] { "w1", "w2" };
            _userRepoMock.Setup(r => r.GetById("u1")).ReturnsAsync(user);

            await _userService.UpdateWarehouses("u1", newWarehouses, "c1");

            Assert.Equal(2, user.WarehouseIds.Count);
            _userRepoMock.Verify(r => r.Update(user), Times.Once);
        }
        

        [Fact]
        public async Task Delete_ShouldRemoveImageAndCallRepository()
        {
            var user = new UserModel { Id = "u1", CompanyId = "c1", PhotoURI = "path/to/photo.jpg" };
            _userRepoMock.Setup(r => r.GetById("u1")).ReturnsAsync(user);

            await _userService.Delete("u1", "c1");

            _imageServiceMock.Verify(i => i.DeleteImage("path/to/photo.jpg"), Times.Once);
            _userRepoMock.Verify(r => r.Delete("u1"), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldHashPassword_WhenNewPasswordProvided()
        {
            var userId = "u1";
            var companyId = "c1";
            var user = new UserModel { Id = userId, CompanyId = companyId, Name = "Old", Email = "old@test.com" };
            var update = new UserUpdate { Password = "SecurePassword123!" };

            _userRepoMock.Setup(r => r.GetById(userId)).ReturnsAsync(user);
            _hasherMock.Setup(h => h.Hash(update.Password)).Returns("new_hash");
            _userRepoMock.Setup(r => r.Update(user)).ReturnsAsync(user);

            await _userService.Update(userId, update, companyId, null);

            Assert.Equal("new_hash", user.PasswordHash);
            _hasherMock.Verify(h => h.Hash(update.Password), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrowArgumentException_WhenPasswordIsWeak()
        {
            var user = new UserModel { Id = "u1", CompanyId = "c1" };
            var update = new UserUpdate { Password = "123" }; 

            _userRepoMock.Setup(r => r.GetById("u1")).ReturnsAsync(user);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.Update("u1", update, "c1", null));
        }

        [Fact]
        public async Task Update_ShouldHandlePhoto_WhenProvided()
        {
            var user = new UserModel { Id = "u1", CompanyId = "c1", PhotoURI = "old.jpg", Email = "old@test.com" };
            var update = new UserUpdate { Name = "New Name" };
            var photoMock = new Mock<IFormFile>();

            _userRepoMock.Setup(r => r.GetById("u1")).ReturnsAsync(user);
            _imageServiceMock.Setup(i => i.SaveImage(photoMock.Object, "User")).ReturnsAsync("new.jpg");
            _userRepoMock.Setup(r => r.Update(user)).ReturnsAsync(user);

            await _userService.Update("u1", update, "c1", photoMock.Object);

            _imageServiceMock.Verify(i => i.DeleteImage("old.jpg"), Times.Once);
            Assert.Equal("new.jpg", user.PhotoURI);
        }

        

        [Fact]
        public async Task RemoveWarehouse_ShouldRemoveIdFromAllUsersInCompany()
        {
            var warehouseId = "w1";
            var companyId = "c1";
            var users = new List<UserModel>
            {
                new UserModel { CompanyId = companyId, WarehouseIds = new List<string> { "w1", "w2" } },
                new UserModel { CompanyId = companyId, WarehouseIds = new List<string> { "w3" } }
            };

            _userRepoMock.Setup(r => r.GetByCompanyId(companyId)).ReturnsAsync(users);

            await _userService.RemoveWarehouse(warehouseId, companyId);

            Assert.Single(users[0].WarehouseIds);
            Assert.DoesNotContain(warehouseId, users[0].WarehouseIds);
        }
    }
}