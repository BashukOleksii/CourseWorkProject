using Moq;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.User;

namespace InventoryManagementSystem_Tests.ViewModels
{
    public class UserDetailViewModelTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserDetailViewModel _viewModel;

        public UserDetailViewModelTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _viewModel = new UserDetailViewModel(_userServiceMock.Object);
        }

        [Fact]
        public async Task LoadUserDetails_ShouldPopulateUserAndWarehouseIds()
        {
            _viewModel.UserId = "user-123";
            var mockUser = new UserResponse
            {
                Id = "user-123",
                Name = "User",
                WarehouseIds = new List<string> { "wh-1", "wh-2" }
            };

            _userServiceMock
                .Setup(s => s.GetUserById("user-123"))
                .ReturnsAsync(mockUser);

            await _viewModel.LoadUserDetails();

            Assert.Equal("User", _viewModel.User.Name);
            Assert.Equal(2, _viewModel.SelectedWarehouseIds.Count);
            Assert.Equal("Призначено складів: 2", _viewModel.WarehouseCountText);
            _userServiceMock.Verify(s => s.GetUserById("user-123"), Times.Once);
        }

        [Theory]
        [InlineData(UserRole.manager, true)]
        [InlineData(UserRole.admin, false)]
        public void IsManager_ShouldReturnCorrectValue_BasedOnRole(UserRole role, bool expectedIsManager)
        {
            _viewModel.User = new UserResponse { UserRole = role };

            Assert.Equal(expectedIsManager, _viewModel.IsManager);
        }

        [Fact]
        public void WarehouseCountText_ShouldReturnDefault_WhenListIsEmpty()
        {
            _viewModel.SelectedWarehouseIds = new List<string>();

            Assert.Equal("Склади не призначено", _viewModel.WarehouseCountText);
        }

        [Fact]
        public async Task OnSelectedWarehouseIdsChanged_ShouldCallUpdateService()
        {
            var userId = "user-123";
            _viewModel.User = new UserResponse { Id = userId };
            var newWarehouseIds = new List<string> { "wh-5" };

            _userServiceMock
                .Setup(s => s.UpdateUserWarehouses(userId, newWarehouseIds))
                .Returns(Task.CompletedTask);

            _userServiceMock
                .Setup(s => s.GetUserById(userId))
                .ReturnsAsync(new UserResponse { Id = userId, WarehouseIds = newWarehouseIds });

            _viewModel.SelectedWarehouseIds = newWarehouseIds;

            await Task.Delay(100);

            _userServiceMock.Verify(s => s.UpdateUserWarehouses(userId, newWarehouseIds), Times.Once);
        }

    }
}