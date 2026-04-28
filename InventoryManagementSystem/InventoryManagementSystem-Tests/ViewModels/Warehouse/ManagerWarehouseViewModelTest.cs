using Moq;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.Helper;
using InventorySystem_Shared.Warehouse;
using InventorySystem_Shared.User;
namespace InventoryManagementSystem_Tests.ViewModels
{
    public class ManagerWarehouseViewModelTest
    {
        private readonly Mock<IWarehouseService> _warehouseServiceMock;
        private readonly Mock<IUserContextService> _userContextMock;
        private readonly ManagerWarehouseViewModel _viewModel;

        public ManagerWarehouseViewModelTest()
        {
            _warehouseServiceMock = new Mock<IWarehouseService>();
            _userContextMock = new Mock<IUserContextService>();
            _viewModel = new ManagerWarehouseViewModel(_warehouseServiceMock.Object, _userContextMock.Object);
        }

        [Fact]
        public async Task LoadManagerWarehouses_ShouldSetIsListEmpty_WhenUserHasNoWarehouses()
        {
            _userContextMock.SetupGet(u => u.CurrentUser)
                           .Returns(new UserResponse { WarehouseIds = new List<string>() });

            await _viewModel.LoadManagerWarehousesCommand.ExecuteAsync(null);

            Assert.True(_viewModel.IsListEmpty);
            Assert.Empty(_viewModel.Warehouses);
            _warehouseServiceMock.Verify(s => s.GetWarehousesByIds(It.IsAny<List<string>>()), Times.Never);
        }

        [Fact]
        public async Task LoadManagerWarehouses_ShouldApplyPaginationCorrectly()
        {
            var ids = new List<string> { "1", "2", "3", "4", "5", "6", "7" }; 
            _userContextMock.SetupGet(u => u.CurrentUser).Returns(new UserResponse { WarehouseIds = ids });

            var mockWarehouses = ids.Select(id => new WarehouseResponse { Id = id, Name = $"Склад {id}" }).ToList();
            _warehouseServiceMock.Setup(s => s.GetWarehousesByIds(ids)).ReturnsAsync(mockWarehouses);

            _viewModel.PageSize = 5; 

            await _viewModel.LoadManagerWarehousesCommand.ExecuteAsync(null);

            Assert.Equal(5, _viewModel.Warehouses.Count); 
            Assert.True(_viewModel.CanGoNext); 
            Assert.False(_viewModel.IsListEmpty);
        }

        [Fact]
        public async Task NextPage_ShouldUpdateCollectionAndCanGoNext()
        {
            var ids = Enumerable.Range(1, 10).Select(i => i.ToString()).ToList();
            _userContextMock.SetupGet(u => u.CurrentUser).Returns(new UserResponse { WarehouseIds = ids });

            var mockWarehouses = ids.Select(id => new WarehouseResponse { Id = id }).ToList();
            _warehouseServiceMock.Setup(s => s.GetWarehousesByIds(ids)).ReturnsAsync(mockWarehouses);

            _viewModel.PageSize = 6;
            await _viewModel.LoadManagerWarehousesCommand.ExecuteAsync(null); 

            _viewModel.NextPageCommand.Execute(null); 

            Assert.Equal(2, _viewModel.CurrentPage);
            Assert.Equal(4, _viewModel.Warehouses.Count); 
            Assert.False(_viewModel.CanGoNext); 
        }

        [Fact]
        public async Task PreviousPage_ShouldWork_OnlyWhenNotOnFirstPage()
        {
            _viewModel.CurrentPage = 2;
            var ids = new List<string> { "1" };
            _userContextMock.SetupGet(u => u.CurrentUser).Returns(new UserResponse { WarehouseIds = ids });
            _warehouseServiceMock.Setup(s => s.GetWarehousesByIds(ids)).ReturnsAsync(new List<WarehouseResponse> { new() });
            await _viewModel.LoadManagerWarehousesCommand.ExecuteAsync(null);

            _viewModel.PreviousPageCommand.Execute(null);

            Assert.Equal(1, _viewModel.CurrentPage);
        }

    }
}