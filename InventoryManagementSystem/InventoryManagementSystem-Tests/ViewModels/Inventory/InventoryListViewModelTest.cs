using Moq;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Inventory;
using System.Collections.ObjectModel;

namespace InventoryManagementSystem_Tests.ViewModels
{
    public class InventoryListViewModelTests
    {
        private readonly Mock<IInventoryService> _serviceMock;
        private readonly InventoryListViewModel _viewModel;

        public InventoryListViewModelTests()
        {
            _serviceMock = new Mock<IInventoryService>();
            _viewModel = new InventoryListViewModel(_serviceMock.Object);
        }

        [Fact]
        public async Task LoadItems_ShouldNotCallService_IfWarehouseIdIsNull()
        {
            _viewModel.WarehouseId = null;

            await _viewModel.LoadItemsCommand.ExecuteAsync(null);

            _serviceMock.Verify(s => s.GetItemsByWarehouse(It.IsAny<string>(), It.IsAny<InventoryQuery>()), Times.Never);
        }

        [Fact]
        public async Task LoadItems_ShouldCorrectlyMapQueryParameters()
        {
            _viewModel.WarehouseId = "wh-777";
            _viewModel.ItemSearchText = "Молоко";
            _viewModel.CurrentPage = 2;

            _serviceMock.Setup(s => s.GetItemsByWarehouse(It.IsAny<string>(), It.IsAny<InventoryQuery>()))
                        .ReturnsAsync(new List<InventoryResponse>());

            await _viewModel.LoadItemsCommand.ExecuteAsync(null);

            _serviceMock.Verify(s => s.GetItemsByWarehouse(
                "wh-777",
                It.Is<InventoryQuery>(q => q.Name == "Молоко" && q.Page == 2 && q.PageSize == 10)),
                Times.Once);
        }

        [Fact]
        public async Task SearchDebounce_ShouldOnlyExecuteOnce_AfterRapidChanges()
        {
            _viewModel.WarehouseId = "wh-1";
            _serviceMock.Setup(s => s.GetItemsByWarehouse(It.IsAny<string>(), It.IsAny<InventoryQuery>()))
                        .ReturnsAsync(new List<InventoryResponse>());

            _viewModel.ItemSearchText = "T";
            _viewModel.ItemSearchText = "Te";
            _viewModel.ItemSearchText = "Tes";
            _viewModel.ItemSearchText = "Test";

            await Task.Delay(600);

            _serviceMock.Verify(s => s.GetItemsByWarehouse(It.IsAny<string>(), It.IsAny<InventoryQuery>()), Times.Between(1,2,Moq.Range.Inclusive));
        }

        [Fact]
        public async Task DeleteItem_ShouldRemoveFromCollection_AfterServiceSuccess()
        {
            var item = new InventoryResponse { Id = "item-1", Name = "Товар для видалення" };
            _viewModel.Items = new ObservableCollection<InventoryResponse> { item };
            _viewModel.WarehouseId = "wh-1";


            await _viewModel.DeleteItemCommand.ExecuteAsync(item);

            _serviceMock.Verify(s => s.DeleteById("item-1"), Times.AtMostOnce);
            Assert.Empty(_viewModel.Items);
        }

        [Fact]
        public async Task Pagination_CanGoNext_ShouldBeTrue_IfTenItemsReturned()
        {
            _viewModel.WarehouseId = "wh-1";
            var tenItems = new List<InventoryResponse>();
            for (int i = 0; i < 10; i++) tenItems.Add(new InventoryResponse());

            _serviceMock.Setup(s => s.GetItemsByWarehouse(It.IsAny<string>(), It.IsAny<InventoryQuery>()))
                        .ReturnsAsync(tenItems);

            await _viewModel.LoadItemsCommand.ExecuteAsync(null);

            Assert.True(_viewModel.CanGoNext);
        }
    }
}