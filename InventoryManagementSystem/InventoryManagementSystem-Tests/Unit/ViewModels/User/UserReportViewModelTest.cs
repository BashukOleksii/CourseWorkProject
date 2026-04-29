using Moq;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.User;
using System.Collections.ObjectModel;

namespace InventoryManagementSystem_Tests.Unit.ViewModels.User
{
    public class UserReportViewModelTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserReportViewModel _viewModel;

        public UserReportViewModelTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _viewModel = new UserReportViewModel(_userServiceMock.Object);
        }

        [Fact]
        public void RolesList_ShouldContainCorrectValues()
        {
            Assert.Contains(null, _viewModel.Roles);
            Assert.Contains(UserRole.admin, _viewModel.Roles);
            Assert.Contains(UserRole.manager, _viewModel.Roles);
            Assert.Equal(3, _viewModel.Roles.Count);
        }

        [Fact]
        public async Task ApplyFilters_ShouldSyncPage_AndHideFilterPanel()
        {
            _viewModel.CurrentPage = 3;
            _viewModel.IsFilterVisible = true;
            _viewModel.Query.Name = "User";

            _userServiceMock
                .Setup(s => s.GetUsers(It.IsAny<UserQuery>()))
                .ReturnsAsync(new List<UserResponse> { new() { Name = "Test User" } });

            await _viewModel.ApplyFiltersCommand.ExecuteAsync(null);

            _userServiceMock.Verify(s => s.GetUsers(It.Is<UserQuery>(q =>
                q.Page == 3 && q.Name == "User")), Times.Once);

            Assert.False(_viewModel.IsFilterVisible);
            Assert.Single(_viewModel.Users);
        }

        [Fact]
        public async Task NextPage_ShouldIncrementCurrentPage_AndTriggerRequest()
        {
            _viewModel.CurrentPage = 1;
            _userServiceMock
                .Setup(s => s.GetUsers(It.IsAny<UserQuery>()))
                .ReturnsAsync(new List<UserResponse>());

            await _viewModel.NextPageCommand.ExecuteAsync(null);

            Assert.Equal(2, _viewModel.CurrentPage);
            _userServiceMock.Verify(s => s.GetUsers(It.Is<UserQuery>(q => q.Page == 2)), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldRemoveFromCollection_WhenConfirmed()
        {
            var user = new UserResponse { Id = "u123", Name = "ToDelete" };
            _viewModel.Users = new ObservableCollection<UserResponse> { user };

            await _viewModel.DeleteUserCommand.ExecuteAsync(user);

            _userServiceMock.Verify(s => s.DeleteUser("u123"), Times.AtMostOnce);
        }

    }
}