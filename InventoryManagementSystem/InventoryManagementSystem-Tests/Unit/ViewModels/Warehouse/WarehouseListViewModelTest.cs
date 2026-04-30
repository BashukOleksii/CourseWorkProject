using Moq;
using Xunit;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Warehouse;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace InventoryManagementSystem_Tests.Unit.ViewModels.Warehouse
{
    public class WarehouseListViewModelTests
    {
        private readonly Mock<IWarehouseService> _warehouseServiceMock;
        private readonly WarehouseListViewModel _viewModel;

        public WarehouseListViewModelTests()
        {
            _warehouseServiceMock = new Mock<IWarehouseService>();
            _viewModel = new WarehouseListViewModel(_warehouseServiceMock.Object);
        }

        [Fact]
        public async Task LoadWarehouses_ShouldCallService_WithCorrectQuery()
        {
            _viewModel.SearchText = "Західний";
            _viewModel.CurrentPage = 2;
            _viewModel.PageSize = 5;

            var mockResult = new List<WarehouseResponse> { new() { Name = "Склад 1" } };
            _warehouseServiceMock
                .Setup(s => s.GetWarehouses(It.IsAny<WarehouseQuery>()))
                .ReturnsAsync(mockResult);

            await _viewModel.LoadWarehousesCommand.ExecuteAsync(null);

            _warehouseServiceMock.Verify(s => s.GetWarehouses(It.Is<WarehouseQuery>(q =>
                q.Name == "Західний" &&
                q.Page == 2 &&
                q.PageSize == 5)), Times.Once);

            Assert.Single(_viewModel.Warehouses);
            Assert.False(_viewModel.CanGoNext); 
        }

        [Fact]
        public async Task SearchText_ShouldResetPage_AndExecuteDebouncedSearch()
        {
            _viewModel.CurrentPage = 3;
            _warehouseServiceMock
                .Setup(s => s.GetWarehouses(It.IsAny<WarehouseQuery>()))
                .ReturnsAsync(new List<WarehouseResponse>());

            _viewModel.SearchText = "Новий пошук";

            Assert.Equal(1, _viewModel.CurrentPage); 

            await Task.Delay(600);

            _warehouseServiceMock.Verify(s => s.GetWarehouses(It.Is<WarehouseQuery>(q =>
                q.Name == "Новий пошук")), Times.Once);
        }

        [Fact]
        public async Task NextPage_ShouldIncrementPage_AndReload()
        {
            _viewModel.CurrentPage = 1;
            _warehouseServiceMock
                .Setup(s => s.GetWarehouses(It.IsAny<WarehouseQuery>()))
                .ReturnsAsync(new List<WarehouseResponse>());

            await _viewModel.NextPageCommand.ExecuteAsync(null);

            Assert.Equal(2, _viewModel.CurrentPage);
            _warehouseServiceMock.Verify(s => s.GetWarehouses(It.Is<WarehouseQuery>(q => q.Page == 2)), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldRemoveFromList_OnSuccess()
        {
            var warehouse = new WarehouseResponse { Id = "wh-1", Name = "Склад на видалення" };
            _viewModel.Warehouses = new ObservableCollection<WarehouseResponse> { warehouse };


            await _viewModel.DeleteCommand.ExecuteAsync(warehouse);

            _warehouseServiceMock.Verify(s => s.DeleteWarehouse("wh-1"), Times.Once);
            Assert.Empty(_viewModel.Warehouses);
        }

        [Fact]
        public async Task CanGoNext_ShouldBeTrue_WhenResultCountEqualsPageSize()
        {
            _viewModel.PageSize = 5;
            var fullPage = new List<WarehouseResponse> { new(), new(), new(), new(), new() };

            _warehouseServiceMock
                .Setup(s => s.GetWarehouses(It.IsAny<WarehouseQuery>()))
                .ReturnsAsync(fullPage);

            await _viewModel.LoadWarehousesCommand.ExecuteAsync(null);

            Assert.True(_viewModel.CanGoNext);
        }
    }
}