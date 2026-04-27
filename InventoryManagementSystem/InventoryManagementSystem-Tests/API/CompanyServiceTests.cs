
using AutoMapper;
using FluentValidation.Results;
using InventorySystem_API.Company.Models;
using InventorySystem_API.Company.Repository;
using InventorySystem_API.Company.Service;
using InventorySystem_API.Company.Validation;
using InventorySystem_Shared.Company;
using Moq;

namespace InventoryManagementSystem_Tests
{
    public class CompanyServiceTests
    {
        private readonly Mock<ICompanyRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CompanyService _companyService;

        public CompanyServiceTests()
        {
            _repositoryMock = new Mock<ICompanyRepository>();
            _mapperMock = new Mock<IMapper>();

            _companyService = new CompanyService(
                _repositoryMock.Object,
                _mapperMock.Object,
                new CompanyValidator()
            );
        }

        [Fact]
        public async Task Create_ShouldReturnCompanyResponse_WhenSuccessful()
        {
            var dto = new CompanyDTO { Name = "Test" };
            var model = new CompanyModel { Id = "1", Name = "Test" };
            var response = new CompanyResponse { Id = "1", Name = "Test" };

            _mapperMock.Setup(m => m.Map<CompanyModel>(dto)).Returns(model);
            _repositoryMock.Setup(r => r.Create(model)).ReturnsAsync(model);
            _mapperMock.Setup(m => m.Map<CompanyResponse>(model)).Returns(response);

            var result = await _companyService.Create(dto);

            Assert.NotNull(result);
            Assert.Equal(response.Id, result.Id);
            _repositoryMock.Verify(r => r.Create(It.IsAny<CompanyModel>()), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnResponse_WhenCompanyExists()
        {
            var id = "1";
            var model = new CompanyModel { Id = id };
            var response = new CompanyResponse { Id = id };

            _repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync(model);
            _mapperMock.Setup(m => m.Map<CompanyResponse>(model)).Returns(response);

            var result = await _companyService.GetById(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNullResponse_WhenCompanyDoesNotExist()
        {
            var id = "non-existent";
            _repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync((CompanyModel?)null);
            _mapperMock.Setup(m => m.Map<CompanyResponse>(null)).Returns((CompanyResponse?)null);

            var result = await _companyService.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task Delete_ShouldCallRepository_WhenCompanyExists()
        {
            var id = "1";
            var model = new CompanyModel { Id = id };
            _repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync(model);

            await _companyService.Delete(id);

            _repositoryMock.Verify(r => r.Delete(id), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrowKeyNotFoundException_WhenCompanyDoesNotExist()
        {
            var id = "invalid";
            _repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync((CompanyModel?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _companyService.Delete(id));
        }

        [Fact]
        public async Task Update_ShouldReturnUpdatedResponse_WhenValid()
        {
            var id = "1";
            var update = new CompanyUpdate { Name = "New Name" };
            var model = new CompanyModel { Id = id, Name = "Old Name", Phone = "+380111111111" };

            var dtoFromMapper = new CompanyDTO
            {
                Name = "New Name",
                Phone = "+380991234567" 
            };
            var response = new CompanyResponse { Id = id, Name = "New Name" };

            _repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync(model);

            _mapperMock.Setup(m => m.Map<CompanyDTO>(It.IsAny<CompanyModel>())).Returns(dtoFromMapper);
            _mapperMock.Setup(m => m.Map<CompanyResponse>(It.IsAny<CompanyModel>())).Returns(response);

            var result = await _companyService.Update(id, update);

            Assert.Equal("New Name", result.Name);
            _repositoryMock.Verify(r => r.Update(It.IsAny<CompanyModel>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrowArgumentException_WhenValidationFails()
        {
            var id = "1";
            var update = new CompanyUpdate { Name = "" };
            var model = new CompanyModel { Id = id, Name = "Old Name" };

            var invalidDto = new CompanyDTO
            {
                Name = "", 
                Phone = "+380000000000" 
            };

            _repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync(model);

            _mapperMock.Setup(m => m.Map<CompanyDTO>(It.IsAny<object>())).Returns(invalidDto);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _companyService.Update(id, update));


            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task Update_ShouldThrowKeyNotFoundException_WhenCompanyDoesNotExist()
        {
            var id = "invalid";
            _repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync((CompanyModel?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _companyService.Update(id, new CompanyUpdate()));
        }
    }
}