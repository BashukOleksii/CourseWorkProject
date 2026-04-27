using Moq;
using AutoMapper;
using InventorySystem_API.Loging.Service;
using InventorySystem_API.Loging.Repository;
using InventorySystem_API.Loging.Models;
using InventorySystem_Shared.Loging;
using MongoDB.Driver;

namespace InventoryManagementSystem_Tests
{
    public class LogServiceTests
    {
        private readonly Mock<ILogRepository> _logRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly LogService _logService;

        public LogServiceTests()
        {
            _logRepoMock = new Mock<ILogRepository>();
            _mapperMock = new Mock<IMapper>();

            _logService = new LogService(
                _logRepoMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Create_ShouldReturnResponse_WhenSuccessful()
        {
            var dto = new AuditLogCreate { UserId = "u1" };
            var model = new AuditLogModel { UserId = "u1" };
            var response = new AuditLogResponse { UserId = "u1" };

            _mapperMock.Setup(m => m.Map<AuditLogModel>(dto)).Returns(model);
            _logRepoMock.Setup(r => r.Create(model)).ReturnsAsync(model);
            _mapperMock.Setup(m => m.Map<AuditLogResponse>(model)).Returns(response);

            var result = await _logService.Create(dto);

            Assert.NotNull(result);
            Assert.Equal("u1", result.UserId);
            _logRepoMock.Verify(r => r.Create(model), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldThrowKeyNotFound_WhenLogMissing()
        {
            _logRepoMock.Setup(r => r.GetById("none")).ReturnsAsync((AuditLogModel?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _logService.GetById("none"));
        }

        [Fact]
        public async Task GetById_ShouldReturnResponse_WhenExists()
        {
            var model = new AuditLogModel { Id = "l1" };
            var response = new AuditLogResponse { Id = "l1" };

            _logRepoMock.Setup(r => r.GetById("l1")).ReturnsAsync(model);
            _mapperMock.Setup(m => m.Map<AuditLogResponse>(model)).Returns(response);

            var result = await _logService.GetById("l1");

            Assert.Equal("l1", result.Id);
        }

        [Fact]
        public async Task Delete_ShouldCallRepository_WhenExists()
        {
            var model = new AuditLogModel { Id = "l1" };
            _logRepoMock.Setup(r => r.GetById("l1")).ReturnsAsync(model);

            await _logService.Delete("l1");

            _logRepoMock.Verify(r => r.Delete("l1"), Times.Once);
        }

        [Fact]
        public async Task Get_ShouldThrowArgumentException_WhenPaginationIsNegative()
        {
            var query = new AuditLogQuery { Page = -1, PageSize = 10 };

            await Assert.ThrowsAsync<ArgumentException>(() => _logService.Get(query, "c1"));
        }

        [Fact]
        public async Task Get_ShouldThrowArgumentException_WhenDateRangeIsInvalid()
        {
            var query = new AuditLogQuery
            {
                From = DateTime.UtcNow,
                To = DateTime.UtcNow.AddDays(-1)
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _logService.Get(query, "c1"));
        }

        [Fact]
        public async Task Get_ShouldApplyFiltersAndReturnList()
        {
            var query = new AuditLogQuery
            {
                UserId = "u1",
                Page = 1,
                PageSize = 10
            };
            var companyId = "c1";
            var models = new List<AuditLogModel> { new AuditLogModel { UserId = "u1" } };
            var responses = new List<AuditLogResponse> { new AuditLogResponse { UserId = "u1" } };

            _logRepoMock.Setup(r => r.Get(It.IsAny<FilterDefinition<AuditLogModel>>(), query.PageSize, query.Page))
                .ReturnsAsync(models);
            _mapperMock.Setup(m => m.Map<List<AuditLogResponse>>(models)).Returns(responses);

            var result = await _logService.Get(query, companyId);

            Assert.Single(result);
            Assert.Equal("u1", result[0].UserId);
            _logRepoMock.Verify(r => r.Get(It.IsAny<FilterDefinition<AuditLogModel>>(), 10, 1), Times.Once);
        }
    }
}