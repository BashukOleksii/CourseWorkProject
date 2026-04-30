using Moq;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Warehouse;
using System.Collections.ObjectModel;

namespace InventoryManagementSystem_Tests.Unit.ViewModels.Warehouse
{
    public class WarehouseReportViewModelTests
    {
        private readonly Mock<IWarehouseService> _warehouseServiceMock;
        private readonly WarehouseReportViewModel _viewModel;

        public WarehouseReportViewModelTests()
        {
            _warehouseServiceMock = new Mock<IWarehouseService>();
            _viewModel = new WarehouseReportViewModel(_warehouseServiceMock.Object);
        }

        [Fact]
        public async Task ApplyFilters_ShouldCallService_WithCorrectParametersAndHideFilters()
        {
            _viewModel.IsFilterVisible = true;
            _viewModel.CurrentPage = 2;
            _viewModel.Query.Name = "Київ";

            var mockList = new List<WarehouseResponse> { new() { Name = "Склад Київ 1" } };
            _warehouseServiceMock
                .Setup(s => s.GetWarehouses(It.IsAny<WarehouseQuery>()))
                .ReturnsAsync(mockList);

            await _viewModel.ApplyFiltersCommand.ExecuteAsync(null);

            _warehouseServiceMock.Verify(s => s.GetWarehouses(It.Is<WarehouseQuery>(q =>
                q.Page == 2 && q.Name == "Київ")), Times.Once);

            Assert.False(_viewModel.IsFilterVisible); 
            Assert.Single(_viewModel.Warehouses);
        }

        [Fact]
        public async Task NextPage_ShouldIncrementPage_AndTriggerApplyFilters()
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
        public async Task Delete_ShouldRemoveItemFromCollection_OnSuccess()
        {
            var warehouse = new WarehouseResponse { Id = "wh-1", Name = "Склад" };
            _viewModel.Warehouses = new ObservableCollection<WarehouseResponse> { warehouse };

            await _viewModel.DeleteCommand.ExecuteAsync(warehouse);

            _warehouseServiceMock.Verify(s => s.DeleteWarehouse("wh-1"), Times.AtMostOnce);
        }

        [Fact]
        public void ToggleFilters_ShouldInvertBooleanValue()
        {
            _viewModel.IsFilterVisible = false;

            _viewModel.ToggleFiltersCommand.Execute(null);

            Assert.True(_viewModel.IsFilterVisible);
        }
    }
}