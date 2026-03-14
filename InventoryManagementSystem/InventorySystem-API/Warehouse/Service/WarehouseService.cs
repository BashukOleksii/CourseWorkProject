using AutoMapper;
using FluentValidation;
using InventorySystem_API.User.Services;
using InventorySystem_API.Warehouse.Models;
using InventorySystem_API.Warehouse.Repository;
using InventorySystem_API.Warehouse.Validation;
using InventorySystem_Shared.Warehouse;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.IdGenerators;

namespace InventorySystem_API.Warehouse.Service
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IUserService _userService;
        private readonly IMapper _warehouseMapper;
        private readonly WarehouseValidator _warehouseValidator;

        public WarehouseService(
            IWarehouseRepository warehouseRepository, 
            IUserService userService, 
            IMapper warehouseMapper, 
            WarehouseValidator warehouseValidator)
        {
            _warehouseRepository = warehouseRepository;
            _userService = userService;
            _warehouseMapper = warehouseMapper;
            _warehouseValidator = warehouseValidator;
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
    }
}
