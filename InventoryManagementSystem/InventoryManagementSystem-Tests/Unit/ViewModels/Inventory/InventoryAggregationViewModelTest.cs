using Moq;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Inventory;

namespace InventoryManagementSystem_Tests.Unit.ViewModels.Inventory
{
    public class InventoryAggregationViewModelTest
    {
        private readonly Mock<IInventoryService> _inventoryServiceMock;
        private readonly InventoryAggregationViewModel _viewModel;

        public InventoryAggregationViewModelTest()
        {
            _inventoryServiceMock = new Mock<IInventoryService>();
            _viewModel = new InventoryAggregationViewModel(_inventoryServiceMock.Object);
            _viewModel.WarehouseId = "wh-123";
        }

        [Fact]
        public async Task LoadData_ShouldCallService_WithCurrentQueryAndHideFilters()
        {
            _viewModel.IsFilterVisible = true;
            _viewModel.Query.Page = 1;
            _viewModel.Query.PageSize = 10;

            _inventoryServiceMock
                .Setup(s => s.GetItemsByWarehouse("wh-123", _viewModel.Query))
                .ReturnsAsync(new List<InventoryResponse> { new() });

            await _viewModel.LoadDataCommand.ExecuteAsync(null);

            _inventoryServiceMock.Verify(s => s.GetItemsByWarehouse("wh-123", It.IsAny<InventoryQuery>()), Times.Once);
            Assert.False(_viewModel.IsFilterVisible); 
            Assert.Single(_viewModel.Items);
        }

        [Fact]
        public async Task ResetFilters_ShouldRestoreDefaultQuery_AndReload()
        {
            _viewModel.Query.Page = 5;
            _viewModel.Query.SortDescending = false;


            _inventoryServiceMock
                .Setup(s => s.GetItemsByWarehouse("wh-123", It.Is<InventoryQuery>(q => q.Page == 1)))
                .ReturnsAsync(new List<InventoryResponse> { new() });

            await _viewModel.ResetFiltersCommand.ExecuteAsync(null);


            Assert.Equal(1, _viewModel.Query.Page);
            Assert.Equal(10, _viewModel.Query.PageSize);
            _inventoryServiceMock.Verify(s => s.GetItemsByWarehouse("wh-123", It.Is<InventoryQuery>(q => q.Page == 1)), Times.Once);
        }



        [Fact]
        public async Task NextPage_ShouldIncrementPageAndReload()
        {
            _viewModel.Query.Page = 1;
            _inventoryServiceMock
                .Setup(s => s.GetItemsByWarehouse(It.IsAny<string>(), It.IsAny<InventoryQuery>()))
                .ReturnsAsync(new List<InventoryResponse>());
            
            await _viewModel.NextPageCommand.ExecuteAsync(null);

            Assert.Equal(2, _viewModel.Query.Page);
            _inventoryServiceMock.Verify(s => s.GetItemsByWarehouse("wh-123", It.Is<InventoryQuery>(q => q.Page == 2)), Times.Once);
        }

        [Fact]
        public void ToggleFilter_ShouldInvertVisibility()
        {
            _viewModel.IsFilterVisible = false;

            _viewModel.ToggleFilterCommand.Execute(null);

            Assert.True(_viewModel.IsFilterVisible);
        }
    }
}