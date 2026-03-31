using AutoMapper;
using FluentValidation;
using InventorySystem_API.Inventory.Repository;
using InventorySystem_API.Inventory.Service;
using InventorySystem_API.User.Services;
using InventorySystem_API.Warehouse.Models;
using InventorySystem_API.Warehouse.Repository;
using InventorySystem_API.Warehouse.Validation;
using InventorySystem_Shared.Warehouse;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace InventorySystem_API.Warehouse.Service
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IMapper _warehouseMapper;
        private readonly WarehouseValidator _warehouseValidator;

        private readonly IUserService _userService;
        private readonly IInventoryService _inventoryService;

        public WarehouseService(
            IWarehouseRepository warehouseRepository, 
            IUserService userService, 
            IMapper warehouseMapper, 
            WarehouseValidator warehouseValidator,
            IInventoryService inventoryService)
        {
            _warehouseRepository = warehouseRepository;
            _userService = userService;
            _warehouseMapper = warehouseMapper;
            _warehouseValidator = warehouseValidator;
            _inventoryService = inventoryService;
        }

        private async Task<WarehouseModel> GetById(string warehouseId, string companyId)
        {
            var model = await _warehouseRepository.GetById(warehouseId);

            if (model is null)
                throw new KeyNotFoundException($"Не знайдено складу із id:{warehouseId}");

            if (model.CompanyId != companyId)
                throw new ArgumentException("Не можна видалити склад іншої компанії");

            return model;
        }

        public async Task<WarehouseResponse> Create(WarehouseDTO dto, string companyId)
        {
            var model = _warehouseMapper.Map<WarehouseModel>(dto);
            model.CompanyId = companyId;

            var modelResponse = await _warehouseRepository.Create(model);
            await _userService.AddWarehouseToAdmins(modelResponse.Id, companyId);

            return _warehouseMapper.Map<WarehouseResponse>(modelResponse);
        }

        public async Task Delete(string id, string companyId)
        {
            var model = await GetById(id, companyId);

            await _userService.RemoveWarehouse(model.Id, model.CompanyId);
            await _inventoryService.DeleteByWarehouseId(model.Id);

            await _warehouseRepository.Delete(model.Id);
        }

        public async Task<List<WarehouseResponse>> GetByIds(string[] id, string companyId)
        {
            var models = await _warehouseRepository.GetByIds(id);

            if (models.Count() < id.Count())
                throw new KeyNotFoundException("Не знайдено всі склади завказаними Id");

            var response = new List<WarehouseResponse>();

            foreach (var model in models) {
                if (model.CompanyId != companyId)
                    throw new ArgumentException("Не можна отримати доступ до складу іншої компанії");

                response.Add(_warehouseMapper.Map<WarehouseResponse>(model));
            }
            
            return response;
           
        }

        public async Task<List<string>> GetIdsByCompanyId(string companyId) =>
            await _warehouseRepository.GetIdsByCompanyId(companyId);
            
        public async Task<WarehouseResponse> Update(string id, WarehouseUpdate update, string companyId)
        {
            var model = await GetById(id, companyId);

            if(update.Name is not null)
                model.Name = update.Name;
            if(update.Description is not null)
                model.Description = update.Description;
            if(update.Address is not null)
                model.Address = update.Address;
            if (update.Area is not null)
                model.Area = update.Area.Value;

            var validationResult = _warehouseValidator.Validate(_warehouseMapper.Map<WarehouseDTO>(model));

            if (!validationResult.IsValid)
                throw new ArgumentException(validationResult.Errors.ToString());

            var updatedModel = await _warehouseRepository.Update(model);

            return _warehouseMapper.Map<WarehouseResponse>(updatedModel);
        }

        async Task<WarehouseResponse> IWarehouseService.GetById(string warehouseId, string companyId)
        {
            var model = await GetById(warehouseId, companyId);

            return _warehouseMapper.Map<WarehouseResponse>(model);
        }

        public async Task<List<WarehouseResponse>> Get(string companyId, WarehouseQuery warehouseQuery)
        {
            if (warehouseQuery.PageSize < 0 || warehouseQuery.Page < 0)
                throw new ArgumentException("Невірні параметри для пагінації");

            if (warehouseQuery.MinArea.HasValue && warehouseQuery.MinArea < 0 ||
                warehouseQuery.MaxArea.HasValue && warehouseQuery.MaxArea < 0 ||
                warehouseQuery.MinArea.HasValue && warehouseQuery.MaxArea.HasValue && warehouseQuery.MaxArea < warehouseQuery.MinArea)
                throw new ArgumentException("Невірно встановлені значення для площі");

            var builder = new FilterDefinitionBuilder<WarehouseModel>();
            var filter = builder.Eq(warehouse => warehouse.CompanyId,companyId);
            
            SortDefinition<WarehouseModel> sort = null;

            if (warehouseQuery.Name is not null)
                filter &= builder.Regex(warehouse => warehouse.Name, new BsonRegularExpression(warehouseQuery.Name, "i"));
            if (warehouseQuery.PartDescription is not null)
                filter &= builder.Regex(warehouse => warehouse.Description, new BsonRegularExpression(warehouseQuery.PartDescription, "i"));
            if (warehouseQuery.MinArea.HasValue)
                filter &= builder.Gt(warehouse => warehouse.Area, warehouseQuery.MinArea);
            if (warehouseQuery.MaxArea.HasValue)
                filter &= builder.Lt(warehouse => warehouse.Area, warehouseQuery.MaxArea);

            if (warehouseQuery.Address is not null)
            {
                var address = warehouseQuery.Address;

                if (address.Name is not null)
                    filter &= builder.Regex(warehouse => warehouse.Address.Name, new BsonRegularExpression(address.Name, "i"));
                if (address.Country is not null)
                    filter &= builder.Eq(warehouse => warehouse.Address.Country,address.Country);
                if (address.State is not null)
                    filter &= builder.Eq(warehouse => warehouse.Address.State, address.State);
                if (address.District is not null)
                    filter &= builder.Eq(warehouse => warehouse.Address.District, address.District);
                if (address.City is not null)
                    filter &= builder.Eq(warehouse => warehouse.Address.City, address.City);
                if (address.Street is not null)
                    filter &= builder.Eq(warehouse => warehouse.Address.Street, address.Street);
                if (address.HouseNumber is not null)
                    filter &= builder.Eq(warehouse => warehouse.Address.HouseNumber, address.HouseNumber);
                if (address.Postcode is not null)
                    filter &= builder.Eq(warehouse => warehouse.Address.Postcode, address.Postcode);

            }

            if(warehouseQuery.SortBy is not null)
            {
                var sortBuilder = Builders<WarehouseModel>.Sort;

                sort = warehouseQuery.OrderByDescending ?
                    sortBuilder.Descending(warehouseQuery.SortBy) :
                    sortBuilder.Ascending(warehouseQuery.SortBy);
            }

            var response = await _warehouseRepository.Get(filter, sort, warehouseQuery.PageSize, warehouseQuery.Page);
            return _warehouseMapper.Map<List<WarehouseResponse>>(response);
        }
    }
}
