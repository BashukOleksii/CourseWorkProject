using AutoMapper;
using InventorySystem_API.Inventory.Models;
using InventorySystem_API.Inventory.Repository;
using InventorySystem_API.Inventory.Validator;
using InventorySystem_API.Service.Image;
using InventorySystem_Shared.Inventory;
using MongoDB.Bson;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;
using static System.Net.WebRequestMethods;

namespace InventorySystem_API.Inventory.Service
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        private readonly IMapper _inventoryMapper;
        private readonly InventoryValidator _validationRules;

        private readonly IImageService _imageService;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IMapper mapper,
            InventoryValidator validationRules,
            IImageService imageService
            )
        {
            _inventoryRepository = inventoryRepository;
            _inventoryMapper = mapper;
            _validationRules = validationRules;
            _imageService = imageService;
        }

        private async Task<InventoryModel> GetModel(string id)
        {
            var model = await _inventoryRepository.GetById(id);

            if (model is null)
                throw new KeyNotFoundException($"Товару із id:{id} не знайдено");

            return model;
        }

        public async Task<InventoryResponse> Create(InventoryCreate dto, string warehouseId, IFormFile? photo)
        {
            var model = _inventoryMapper.Map<InventoryModel>(dto);
            model.WarehouseId = warehouseId;

            if (photo is null)
                model.PhotoURI = _imageService.GetDefaultImage("Inventory");
            else
                model.PhotoURI = await _imageService.SaveImage(photo ,"Inventory");

            var response = await _inventoryRepository.Create(model);

            return _inventoryMapper.Map<InventoryResponse>(response);
        }

        public async Task DeleteById(string id)
        {
            var model = await GetModel(id);

            _imageService.DeleteImage(model.PhotoURI);
            await _inventoryRepository.DeleteById(model.Id);
        }

        public async Task DeleteByWarehouseId(string warehouseId) {
            var modelList = await _inventoryRepository.GetByWarehouseId(warehouseId);

            if (modelList.Count() == 0)
                throw new ArgumentException($"Не знадено товарів за вказаним id складу:{warehouseId}");

            foreach (var model in modelList)
                _imageService.DeleteImage(model.PhotoURI);

            await _inventoryRepository.DeleteByWarehouseId(warehouseId);
        }
        

        public async Task<List<InventoryResponse>> Get(InventoryQuery inventoryQuery, string warehouseId)
        {
            var builder = new FilterDefinitionBuilder<InventoryModel>();
            var filter = builder.Eq(inventory => inventory.WarehouseId, warehouseId);

            if(inventoryQuery.Name is not null)
                 filter &= builder.Eq(inventory => inventory.Name, inventoryQuery.Name);
            if (inventoryQuery.Description is not null)
                filter &= builder.Regex(inventory => inventory.Description, new BsonRegularExpression(inventoryQuery.Description, "i"));

            if(inventoryQuery.Manufacturer is not null)
            {
                var manufacturer = inventoryQuery.Manufacturer;

                if(manufacturer.Name is not null)
                    filter &= builder.Regex(inventory => inventory.Manufacturer.Name, new BsonRegularExpression(manufacturer.Name, "i"));
                if (manufacturer.Country is not null)
                    filter &= builder.Regex(inventory => inventory.Manufacturer.Country, new BsonRegularExpression(manufacturer.Country, "i"));
            }

            if (inventoryQuery.InventoryType is not null)
                filter &= builder.Eq(inventory => inventory.InventoryType,inventoryQuery.InventoryType);

            if (inventoryQuery.MinPrice is not null)
                filter &= builder.Gt(inventory => inventory.Price, inventoryQuery.MinPrice);
            if (inventoryQuery.MaxPrice is not null)
                filter &= builder.Lt(inventory => inventory.Price, inventoryQuery.MaxPrice);

            if (inventoryQuery.MinQuantity is not null)
                filter &= builder.Gt(inventory => inventory.Quantity, inventoryQuery.MinQuantity);
            if (inventoryQuery.MaxQuantity is not null)
                filter &= builder.Lt(inventory => inventory.Quantity, inventoryQuery.MaxQuantity);

            var response = await _inventoryRepository.Get(filter, inventoryQuery.PageSize, inventoryQuery.Page);

            return _inventoryMapper.Map<List<InventoryResponse>>(response);
        }

        public async Task<InventoryResponse> GetById(string id)
        {
            var model = await GetModel(id);

            return _inventoryMapper.Map<InventoryResponse>(model);
        }

        public async Task<InventoryResponse> Update(string id, InventoryUpdate dto, IFormFile? photo)
        {
            var model = await GetModel(id);

            if (dto.Name is not null)
                model.Name = dto.Name;
            if (dto.Description is not null)
                model.Description = dto.Description;

            if (dto.Manufacturer is not null)
            {
                var manufacturer = dto.Manufacturer;

                if (manufacturer.Name is not null)
                    model.Manufacturer.Name = manufacturer.Name;
                if (manufacturer.Country is not null)
                    model.Manufacturer.Country = manufacturer.Country;
            }

            if (dto.InventoryType is not null)
                model.InventoryType = dto.InventoryType.Value;

            if(dto.Price is not null)
                model.Price = dto.Price.Value;

            if(dto.Quantity is not null)
                model.Quantity = dto.Quantity.Value;

            if (dto.CustomFileds is not null)
                model.CustomFileds = dto.CustomFileds;

            var validationResult = _validationRules.Validate(_inventoryMapper.Map<InventoryCreate>(model));

            if (!validationResult.IsValid)
                throw new ArgumentException(string.Join(";", validationResult.Errors.Select(e => e.ErrorMessage)));

            if(photo is not null)
            {
                _imageService.DeleteImage(model.PhotoURI);
                model.PhotoURI = await _imageService.SaveImage(photo,"Inventory");
            }

            var response = await _inventoryRepository.Update(model);

            return _inventoryMapper.Map<InventoryResponse>(response);
        }
    }
}
