using Moq;
using Xunit;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Inventory.Manufacturer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem_Tests.ViewModels
{
    public class ManufacturerListViewModelTest
    {
        private readonly Mock<IManufacturerService> _serviceMock;
        private readonly ManufacturerListViewModel _viewModel;
        private readonly List<InventoryManufacturer> _testData;

        public ManufacturerListViewModelTest()
        {
            _serviceMock = new Mock<IManufacturerService>();

            _testData = new List<InventoryManufacturer>
            {
                new() { Name = "Samsung", Country = "South Korea" },
                new() { Name = "Apple", Country = "USA" },
                new() { Name = "Sony", Country = "Japan" },
                new() { Name = "SoftServe", Country = "Ukraine" },
                new() { Name = "Ajax", Country = "Ukraine" }
            };

            _serviceMock.Setup(s => s.GetManufacturersAsync()).ReturnsAsync(_testData);
            _viewModel = new ManufacturerListViewModel(_serviceMock.Object);
        }

        [Fact]
        public async Task LoadData_ShouldPopulateList()
        {
            await _viewModel.LoadDataCommand.ExecuteAsync(null);

            Assert.Equal(5, _viewModel.DisplayedManufacturers.Count);
            Assert.False(_viewModel.IsListEmpty);
        }

        [Fact]
        public async Task SearchText_ShouldFilterByNameAndCountry()
        {
            await _viewModel.LoadDataCommand.ExecuteAsync(null);

            _viewModel.SearchText = "Ukraine";

            Assert.Equal(2, _viewModel.DisplayedManufacturers.Count);
            Assert.All(_viewModel.DisplayedManufacturers, m => Assert.Contains("Ukraine", m.Country));
        }

        [Fact]
        public async Task SearchText_ShouldResetToFirstPage()
        {
            await _viewModel.LoadDataCommand.ExecuteAsync(null);
            _viewModel.CurrentPage = 2;

            _viewModel.SearchText = "S";

            Assert.Equal(1, _viewModel.CurrentPage);
        }

        [Fact]
        public async Task Delete_ShouldRemoveItemAndSave()
        {
            await _viewModel.LoadDataCommand.ExecuteAsync(null);
            var itemToDelete = _testData.First();

            await _viewModel.DeleteCommand.ExecuteAsync(itemToDelete);

            _serviceMock.Verify(s => s.SaveManufacturersAsync(It.Is<List<InventoryManufacturer>>(l => l.Count == 4)), Times.Once);
            Assert.DoesNotContain(itemToDelete, _viewModel.DisplayedManufacturers);
        }

        [Fact]
        public async Task Pagination_ShouldUpdateCorrectly_WithPageSize()
        {
            _viewModel.PageSize = 2;
            await _viewModel.LoadDataCommand.ExecuteAsync(null); 

            Assert.Equal(2, _viewModel.DisplayedManufacturers.Count);
            Assert.True(_viewModel.CanGoNext);

            _viewModel.NextPageCommand.Execute(null);

            Assert.Equal(2, _viewModel.DisplayedManufacturers.Count);

            _viewModel.NextPageCommand.Execute(null);

            Assert.Single(_viewModel.DisplayedManufacturers); 
            Assert.False(_viewModel.CanGoNext);
        }

        [Fact]
        public async Task ApplyFilters_ShouldSetIsListEmpty_WhenNoMatches()
        {
            await _viewModel.LoadDataCommand.ExecuteAsync(null);

            _viewModel.SearchText = "NonExistentManufacturer";

            Assert.Empty(_viewModel.DisplayedManufacturers);
            Assert.True(_viewModel.IsListEmpty);
        }
    }
}