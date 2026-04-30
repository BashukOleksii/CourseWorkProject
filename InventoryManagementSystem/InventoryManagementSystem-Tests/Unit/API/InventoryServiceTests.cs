using AutoMapper;
using InventorySystem_API.Inventory.Models;
using InventorySystem_API.Inventory.Repository;
using InventorySystem_API.Inventory.Service;
using InventorySystem_API.Inventory.Validator;
using InventorySystem_API.Service.Image;
using InventorySystem_Shared.Inventory;
using InventorySystem_Shared.Inventory.Manufacturer;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Moq;
using System.Text;
using System.Text.Json;

namespace InventoryManagementSystem_Tests.Unit.API
{
    public class InventoryServiceTests
    {
        private readonly Mock<IInventoryRepository> _inventoryRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IImageService> _imageServiceMock;
        private readonly InventoryValidator _validator;
        private readonly InventoryService _inventoryService;

        public InventoryServiceTests()
        {
            _inventoryRepoMock = new Mock<IInventoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _imageServiceMock = new Mock<IImageService>();
            _validator = new InventoryValidator();

            _inventoryService = new InventoryService(
                _inventoryRepoMock.Object,
                _mapperMock.Object,
                _validator,
                _imageServiceMock.Object
            );
        }

        [Fact]
        public async Task Create_ShouldAssignDefaultImage_WhenPhotoIsNull()
        {
            var dto = new InventoryCreate { Name = "Fridge" };
            var warehouseId = "w1";
            var model = new InventoryModel { Name = "Fridge" };
            var response = new InventoryResponse { Name = "Fridge" };

            _mapperMock.Setup(m => m.Map<InventoryModel>(dto)).Returns(model);
            _imageServiceMock.Setup(i => i.GetDefaultImage("Inventory")).Returns("default.png");
            _inventoryRepoMock.Setup(r => r.Create(model)).ReturnsAsync(model);
            _mapperMock.Setup(m => m.Map<InventoryResponse>(model)).Returns(response);

            var result = await _inventoryService.Create(dto, warehouseId, null);

            Assert.Equal("default.png", model.PhotoURI);
            Assert.Equal(warehouseId, model.WarehouseId);
            _inventoryRepoMock.Verify(r => r.Create(model), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldSavePhoto_WhenPhotoIsProvided()
        {
            var dto = new InventoryCreate { Name = "Fridge" };
            var photoMock = new Mock<IFormFile>();
            var model = new InventoryModel { Name = "Fridge" };

            _mapperMock.Setup(m => m.Map<InventoryModel>(dto)).Returns(model);
            _imageServiceMock.Setup(i => i.SaveImage(photoMock.Object, "Inventory")).ReturnsAsync("saved_photo.png");
            _inventoryRepoMock.Setup(r => r.Create(model)).ReturnsAsync(model);

            await _inventoryService.Create(dto, "w1", photoMock.Object);

            Assert.Equal("saved_photo.png", model.PhotoURI);
        }

        [Fact]
        public async Task DeleteById_ShouldThrowKeyNotFound_WhenInventoryMissing()
        {
            _inventoryRepoMock.Setup(r => r.GetById("none")).ReturnsAsync((InventoryModel?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _inventoryService.DeleteById("none"));
        }

        [Fact]
        public async Task DeleteById_ShouldRemoveImageAndRecord_WhenExists()
        {
            var model = new InventoryModel { Id = "i1", PhotoURI = "img.png" };
            _inventoryRepoMock.Setup(r => r.GetById("i1")).ReturnsAsync(model);

            await _inventoryService.DeleteById("i1");

            _imageServiceMock.Verify(i => i.DeleteImage("img.png"), Times.Once);
            _inventoryRepoMock.Verify(r => r.DeleteById("i1"), Times.Once);
        }

        [Fact]
        public async Task DeleteByWarehouseId_ShouldDoNothing_WhenNoItemsFound()
        {
            _inventoryRepoMock.Setup(r => r.GetByWarehouseId("w1")).ReturnsAsync(new List<InventoryModel>());

            await _inventoryService.DeleteByWarehouseId("w1");

            _imageServiceMock.Verify(i => i.DeleteImage(It.IsAny<string>()), Times.Never);
            _inventoryRepoMock.Verify(r => r.DeleteByWarehouseId("w1"), Times.Never);
        }

        [Fact]
        public async Task DeleteByWarehouseId_ShouldRemoveAllImages_WhenItemsExist()
        {
            var models = new List<InventoryModel>
            {
                new InventoryModel { PhotoURI = "1.png" },
                new InventoryModel { PhotoURI = "2.png" }
            };
            _inventoryRepoMock.Setup(r => r.GetByWarehouseId("w1")).ReturnsAsync(models);

            await _inventoryService.DeleteByWarehouseId("w1");

            _imageServiceMock.Verify(i => i.DeleteImage(It.IsAny<string>()), Times.Exactly(2));
            _inventoryRepoMock.Verify(r => r.DeleteByWarehouseId("w1"), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnResponse_WhenExists()
        {
            var model = new InventoryModel { Id = "i1" };
            var response = new InventoryResponse { Id = "i1" };
            _inventoryRepoMock.Setup(r => r.GetById("i1")).ReturnsAsync(model);
            _mapperMock.Setup(m => m.Map<InventoryResponse>(model)).Returns(response);

            var result = await _inventoryService.GetById("i1");

            Assert.Equal("i1", result.Id);
        }

        [Fact]
        public async Task Update_ShouldThrowArgumentException_WhenValidationFails()
        {
            var model = new InventoryModel { Id = "i1", Name = "Old" };
            var update = new InventoryUpdate { Name = "" };
            var invalidCreateDto = new InventoryCreate { Name = "" };

            _inventoryRepoMock.Setup(r => r.GetById("i1")).ReturnsAsync(model);
            _mapperMock.Setup(m => m.Map<InventoryCreate>(It.IsAny<InventoryModel>())).Returns(invalidCreateDto);

            await Assert.ThrowsAsync<ArgumentException>(() => _inventoryService.Update("i1", update, null));
        }

        [Fact]
        public async Task Update_ShouldReplaceImage_WhenNewPhotoProvided()
        {
            var model = new InventoryModel { Id = "i1", PhotoURI = "old.png", Manufacturer = new InventoryManufacturer() };
            var update = new InventoryUpdate { Name = "New" };
            var photoMock = new Mock<IFormFile>();
            var validCreateDto = new InventoryCreate { Name = "New", Manufacturer = new InventoryManufacturer(), Description = "Description", Price=100 };

            _inventoryRepoMock.Setup(r => r.GetById("i1")).ReturnsAsync(model);
            _mapperMock.Setup(m => m.Map<InventoryCreate>(It.IsAny<InventoryModel>())).Returns(validCreateDto);
            _imageServiceMock.Setup(i => i.SaveImage(photoMock.Object, "Inventory")).ReturnsAsync("new.png");
            _inventoryRepoMock.Setup(r => r.Update(model)).ReturnsAsync(model);

            await _inventoryService.Update("i1", update, photoMock.Object);

            _imageServiceMock.Verify(i => i.DeleteImage("old.png"), Times.Once);
            Assert.Equal("new.png", model.PhotoURI);
        }

        [Fact]
        public async Task Import_ShouldThrowArgumentException_WhenFileIsInvalidFormat()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("data.txt");
            fileMock.Setup(f => f.Length).Returns(100);

            await Assert.ThrowsAsync<ArgumentException>(() => _inventoryService.Import("w1", fileMock.Object));
        }

        [Fact]
        public async Task Import_ShouldProcessJsonAndSave_WhenValid()
        {
            var warehouseId = "w1";
            var items = new List<InventoryResponse> { new InventoryResponse { Name = "Imported" } };
            var json = JsonSerializer.Serialize(items);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("data.json");
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            var model = new InventoryModel { Name = "Imported" };
            var createDto = new InventoryCreate { Name = "Imported" };

            _mapperMock.Setup(m => m.Map<InventoryModel>(It.IsAny<InventoryResponse>())).Returns(model);
            _mapperMock.Setup(m => m.Map<InventoryCreate>(It.IsAny<InventoryModel>())).Returns(createDto);
            _imageServiceMock.Setup(i => i.CopyImage(It.IsAny<string>())).ReturnsAsync("copied_img.png");

            await _inventoryService.Import(warehouseId, fileMock.Object);

            _inventoryRepoMock.Verify(r => r.CreateMany(It.IsAny<List<InventoryModel>>()), Times.Once);
        }
    }
}