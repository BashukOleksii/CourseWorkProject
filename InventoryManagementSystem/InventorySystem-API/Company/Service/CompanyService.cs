using AutoMapper;
using InventorySystem_API.Company.Models;
using InventorySystem_API.Company.Repository;
using InventorySystem_API.Company.Validation;
using InventorySystem_Shared.Company;

namespace InventorySystem_API.Company.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _companyMapper;
        private readonly CompanyValidator _compnyValidator;

        public CompanyService(
            ICompanyRepository companyRepository,
            IMapper companyMapper,
            CompanyValidator compnyValidator)
        {
            _companyRepository = companyRepository;
            _companyMapper = companyMapper;
            _compnyValidator = compnyValidator;
        }

        private async Task<CompanyModel> GetModel(string id)
        {
            var company = await _companyRepository.GetById(id);

            if (company is null)
                throw new KeyNotFoundException($"Компанію із вказаним id:{id} не знайдено");

            return company;
        }

        public async Task<CompanyResponse> Create(CompanyDTO companyDTO)
        {
            var companyModel = await _companyRepository.Create(_companyMapper.Map<CompanyModel>(companyDTO));
            return _companyMapper.Map<CompanyResponse>(companyModel);
        }

        public async Task DeleteById(string companyId)
        {
            var companyModel = await GetModel(companyId);

            await _companyRepository.Delete(companyModel.Id);
        }

        public async Task<CompanyResponse> GetById(string companyId)
        {
            var companyModel = await _companyRepository.GetById(companyId);
            return _companyMapper.Map<CompanyResponse>(companyModel);
        }

        public async Task<CompanyResponse> Update(string companyId, CompanyUpdate companyUpdate)
        {
            var companyModel = await GetModel(companyId);

            if(companyUpdate.Name is not null)
                companyModel.Name = companyUpdate.Name;
            if(companyUpdate.Description is not null)
                companyModel.Description = companyUpdate.Description;
            if(companyUpdate.Address is not null)
                companyModel.Address = companyUpdate.Address;
            if(companyUpdate.Phone is not null)
                companyModel.Phone = companyUpdate.Phone;

            var companyDTO = _companyMapper.Map<CompanyDTO>(companyModel);
            var validationResult = _compnyValidator.Validate(companyDTO);

            if (!validationResult.IsValid)
                throw new ArgumentException(validationResult.Errors.ToString());

            await _companyRepository.Update(companyModel);

            return _companyMapper.Map<CompanyResponse>(companyModel);
        }
    }
}
