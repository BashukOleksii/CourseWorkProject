using Moq;
using InventorySystem_MAUI.ViewModel;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Loging;

namespace InventoryManagementSystem_Tests.Unit.ViewModels.Logining
{
    public class LogViewModelTests
    {
        private readonly Mock<ILogService> _logServiceMock;
        private readonly LogViewModel _viewModel;

        public LogViewModelTests()
        {
            _logServiceMock = new Mock<ILogService>();
            _logServiceMock
                .Setup(s => s.GetLogs(It.IsAny<AuditLogQuery>()))
                .ReturnsAsync(new List<AuditLogResponse>());

            _viewModel = new LogViewModel(_logServiceMock.Object);
        }

        [Fact]
        public async Task ApplyFilters_ShouldCombineDateAndTimeCorrectly()
        {
            var testDate = new DateTime(2024, 5, 20);
            _viewModel.Query.From = testDate;
            _viewModel.FromTime = new TimeSpan(14, 30, 0); 

            _viewModel.Query.To = testDate;
            _viewModel.ToTime = new TimeSpan(18, 0, 0); 

            await _viewModel.ApplyFiltersCommand.ExecuteAsync(null);

            _logServiceMock.Verify(s => s.GetLogs(It.Is<AuditLogQuery>(q =>
                q.From == new DateTime(2024, 5, 20, 14, 30, 0) &&
                q.To == new DateTime(2024, 5, 20, 18, 0, 0)
            )), Times.AtLeastOnce);
        }

        [Fact]
        public void ResetFilters_ShouldRestoreDefaultTimeBoundaries()
        {
            _viewModel.FromTime = new TimeSpan(10, 0, 0);
            _viewModel.CurrentPage = 5;

            _viewModel.ResetFiltersCommand.Execute(null);

            Assert.Equal(new TimeSpan(0, 0, 0), _viewModel.FromTime);
            Assert.Equal(new TimeSpan(23, 59, 59), _viewModel.ToTime);
            Assert.Equal(1, _viewModel.CurrentPage);
        }

        [Fact]
        public async Task NextPage_ShouldIncrementPage_AndHideFilters()
        {
            _viewModel.CurrentPage = 1;
            _viewModel.IsFilterVisible = true;

            await _viewModel.NextPageCommand.ExecuteAsync(null);

            Assert.Equal(2, _viewModel.CurrentPage);
            Assert.False(_viewModel.IsFilterVisible);
            _logServiceMock.Verify(s => s.GetLogs(It.Is<AuditLogQuery>(q => q.Page == 2)), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ApplyFilters_ShouldSetCanGoNext_BasedOnPageSize()
        {
            _viewModel.Query.PageSize = 10;
            var fullPage = new List<AuditLogResponse>();
            for (int i = 0; i < 10; i++) fullPage.Add(new AuditLogResponse());

            _logServiceMock
                .Setup(s => s.GetLogs(It.IsAny<AuditLogQuery>()))
                .ReturnsAsync(fullPage);

            await _viewModel.ApplyFiltersCommand.ExecuteAsync(null);

            Assert.True(_viewModel.CanGoNext);
        }

        [Fact]
        public void OptionsLists_ShouldIncludeNullForResettingFilters()
        {
            Assert.Contains(null, _viewModel.ActionOptions);
            Assert.Contains(null, _viewModel.EntityOptions);
            Assert.Contains(null, _viewModel.RoleOptions);
        }

    }
}