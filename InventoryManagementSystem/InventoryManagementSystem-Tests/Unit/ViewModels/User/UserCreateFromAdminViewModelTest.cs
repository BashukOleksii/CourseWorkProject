using Moq;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.Helper;
using InventorySystem_Shared.User;

namespace InventoryManagementSystem_Tests.Unit.ViewModels
{
    public class UserCreateFromAdminViewModelTest
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IUserContextService> _userContextMock;
        private readonly UserCreateFromAdminViewModel _viewModel;

        public UserCreateFromAdminViewModelTest()
        {
            _authServiceMock = new Mock<IAuthService>();
            _userContextMock = new Mock<IUserContextService>();

            _userContextMock.SetupGet(x => x.CurrentUser)
                .Returns(new UserResponse { CompanyId = "comp-777" });

            _viewModel = new UserCreateFromAdminViewModel(_authServiceMock.Object, _userContextMock.Object);
        }

        [Fact]
        public void InitialState_ShouldBeCorrect()
        {
            Assert.Equal(UserRole.manager, _viewModel.SelectedRole);
            Assert.True(_viewModel.CanSelectWarehouses);
            Assert.Empty(_viewModel.SelectedWarehouseIds);
        }

        [Theory]
        [InlineData(UserRole.admin, false)]
        [InlineData(UserRole.manager, true)]
        public void SelectedRole_Change_ShouldUpdateCanSelectWarehouses(UserRole role, bool expectedCanSelect)
        {
            _viewModel.SelectedRole = role;
            
            Assert.Equal(expectedCanSelect, _viewModel.CanSelectWarehouses);
        }

        [Fact]
        public async Task Save_ShouldNotCallAuth_WhenFieldsAreMissing()
        {
            _viewModel.Name = "";

            await _viewModel.SaveCommand.ExecuteAsync(null);

            _authServiceMock.Verify(x => x.Register(It.IsAny<UserRegister>(), It.IsAny<FileResult>()), Times.Never);
        }

        [Fact]
        public async Task Save_ShouldClearWarehouseIds_WhenRoleIsAdmin()
        {
            _viewModel.Name = "New Admin";
            _viewModel.Email = "admin@test.com";
            _viewModel.Password = "123456";
            _viewModel.SelectedRole = UserRole.admin;
            _viewModel.SelectedWarehouseIds = new List<string> { "wh-1", "wh-2" };

            await _viewModel.SaveCommand.ExecuteAsync(null);

            _authServiceMock.Verify(x => x.Register(It.Is<UserRegister>(r =>
                r.UserRole == UserRole.admin &&
                r.WarehouseIds == null &&
                r.CompanyId == "comp-777"),
                It.IsAny<FileResult>()), Times.Once);
        }

        [Fact]
        public async Task Save_ShouldKeepWarehouseIds_WhenRoleIsManager()
        {
            _viewModel.Name = "Manager";
            _viewModel.Email = "m@test.com";
            _viewModel.Password = "123";
            _viewModel.SelectedRole = UserRole.manager;
            _viewModel.SelectedWarehouseIds = new List<string> { "wh-1" };

            await _viewModel.SaveCommand.ExecuteAsync(null);

            _authServiceMock.Verify(x => x.Register(It.Is<UserRegister>(r =>
                r.UserRole == UserRole.manager &&
                r.WarehouseIds.Count == 1),
                It.IsAny<FileResult>()), Times.Once);
        }
    }
}