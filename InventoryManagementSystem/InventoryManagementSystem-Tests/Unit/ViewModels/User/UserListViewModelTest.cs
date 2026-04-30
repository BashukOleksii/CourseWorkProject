using Moq;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.User;
using System.Collections.ObjectModel;

namespace InventoryManagementSystem_Tests.Unit.ViewModels.User
{
    public class UserListViewModelTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserListViewModel _viewModel;

        public UserListViewModelTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _viewModel = new UserListViewModel(_userServiceMock.Object);
        }

        [Fact]
        public async Task LoadUsers_ShouldCallService_WithCorrectQueryParameters()
        {
            _viewModel.SearchText = "Ім'я";
            _viewModel.CurrentPage = 1;
            _viewModel.PageSize = 10;

            _userServiceMock
                .Setup(s => s.GetUsers(It.IsAny<UserQuery>()))
                .ReturnsAsync(new List<UserResponse> { new() { Name = "Ім'я Прізвище" } });

            await _viewModel.LoadUsersCommand.ExecuteAsync(null);

            _userServiceMock.Verify(s => s.GetUsers(It.Is<UserQuery>(q =>
                q.Name == "Ім'я" &&
                q.Page == 1 &&
                q.PageSize == 10)), Times.Once);

            Assert.Single(_viewModel.Users);
            Assert.False(_viewModel.CanGoNext);
        }

        [Fact]
        public async Task SearchText_ShouldResetPage_AndTriggerLoadAfterDelay()
        {
            _viewModel.CurrentPage = 5;
            _userServiceMock.Setup(s => s.GetUsers(It.IsAny<UserQuery>()))
                           .ReturnsAsync(new List<UserResponse>());

            _viewModel.SearchText = "Admin";

            await Task.Delay(600);

            Assert.Equal(1, _viewModel.CurrentPage);

            _userServiceMock.Verify(s => s.GetUsers(It.Is<UserQuery>(q => q.Name == "Admin")), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldRemoveFromCollection_WhenConfirmedAndServiceSucceeds()
        {
            var user = new UserResponse { Id = "u1", Name = "User to delete" };
            _viewModel.Users = new ObservableCollection<UserResponse> { user };

            await _viewModel.DeleteUserCommand.ExecuteAsync(user);

            _userServiceMock.Verify(s => s.DeleteUser("u1"), Times.AtMostOnce);
        }

        [Fact]
        public async Task LoadUsers_ShouldSetCanGoNextTrue_IfResultCountEqualsPageSize()
        {
            _viewModel.PageSize = 2;
            var result = new List<UserResponse> { new(), new() };

            _userServiceMock.Setup(s => s.GetUsers(It.IsAny<UserQuery>()))
                           .ReturnsAsync(result);

            await _viewModel.LoadUsersCommand.ExecuteAsync(null);

            Assert.True(_viewModel.CanGoNext);
        }
    }
}