using AutoMapper;
using InventorySystem_API.Service.Image;
using InventorySystem_API.User.Model;
using InventorySystem_API.User.Repositories;
using InventorySystem_API.User.Validator;
using InventorySystem_Shared.User;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace InventorySystem_API.User.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserModelValidator _validationRules;
        private readonly BCryptPasswordHasher _passwordHasher;
        private readonly IImageService _imageService;
        
        public UserService(
            IUserRepository userRepository, 
            IMapper mapper,
            UserModelValidator validationRules,
            BCryptPasswordHasher bCryptPasswordHasher,
            IImageService imageService
            )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _validationRules = validationRules;
            _passwordHasher = bCryptPasswordHasher;
            _imageService = imageService;
        }

        private async Task<UserModel> GetById(string userId, string companyIdClient)
        {
            var user = await _userRepository.GetById(userId);

            if (user is null)
                throw new KeyNotFoundException($"Користувача із {userId} не знайдено");

            if (user!.CompanyId != companyIdClient)
                throw new ArgumentException("Це не працівник вашої фірми");

            return user;
        }

        public async Task AddWarehouses(string userId, string[] warehouses, string companyIdClient)
        {
            var user = await GetById(userId, companyIdClient);

            if(user.WarehouseIds is null)
                user.WarehouseIds = new List<string>();

            user.WarehouseIds.AddRange(warehouses);

            await _userRepository.Update(user);
        }

        public async Task Delete(string id, string companyIdClient)
        {
            var user = await GetById(id, companyIdClient);

            _imageService.DeleteImage(user.PhotoURI);

            await _userRepository.Delete(user.Id);
        }

        public async Task<List<UserResponse>> Get(string companyIdClient, UserQuery? userQuery)
        {
            var builder = new FilterDefinitionBuilder<UserModel>();
            var filter = builder.Empty;

            SortDefinition<UserModel>? sort = null;

            int? pageSize = null;
            int? page = null;

            if (userQuery is not null)
            {
                if (userQuery.Page < 0 || userQuery.PageSize < 0)
                    throw new ArgumentException("Значенн для пагінації не ожуть бути менше нуля");

                filter &= builder.Eq(user => user.CompanyId, companyIdClient);

                if (userQuery.Name is not null)
                    filter &= builder.Regex(user => user.Name, new MongoDB.Bson.BsonRegularExpression(userQuery.Name, "i"));
                if (userQuery.UserRole is not null)
                    filter &= builder.Eq(user => user.UserRole, userQuery.UserRole);
                if (userQuery.WarehouseId is not null)
                    filter &= builder.AnyEq(user => user.WarehouseIds, userQuery.WarehouseId);

                if(userQuery.SortBy is not null)
                {
                    var sortBuilder = Builders<UserModel>.Sort;

                    sort = userQuery.SortDescending ?
                        sort.Descending(userQuery.SortBy):
                        sort.Ascending(userQuery.SortBy);
                }

                pageSize = userQuery.PageSize; 
                page = userQuery.Page;
            }

            var users = await _userRepository.Get(filter, sort, pageSize, page);

           return _mapper.Map<List<UserResponse>>(users);
        }


        public async Task RemoveWarehouses(string userId, string[] warehouses, string companyIdClient)
        {
            var user = await GetById(userId, companyIdClient);

            foreach(var warehouse in warehouses)
            {
                if (user.WarehouseIds is null || !user.WarehouseIds.Contains(warehouse))
                    throw new ArgumentException($"Працівник не має складу із id:{warehouse}"); 

                user.WarehouseIds.Remove(warehouse);
            }

            await _userRepository.Update(user);

        }

        public async Task<UserResponse> Update(string userId, UserUpdate userUpdate, string companyIdClient, IFormFile? photo)
        {
            var user = await GetById(userId, companyIdClient);

            if(userUpdate.Name is not null)
                user.Name = userUpdate.Name;
            if (userUpdate.Email is not null)
                user.Email = userUpdate.Email;

            if(userUpdate.Password is not null)
            {
                if (!Regex.IsMatch(userUpdate.Password,
                (@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&_])[A-Za-z\d@$!%*?&_]{8,}$")))
                    throw new ArgumentException("Оновлений пароль не відповідає вимогам");

                user.PasswordHash = _passwordHasher.HashPassword(userUpdate.Password);
            }

            var validationResult = _validationRules.Validate(user);

            if (!validationResult.IsValid)
                throw new ArgumentException(validationResult.ToString());

            if (photo is not null)
            {
                _imageService.DeleteImage(user.PhotoURI);
                user.PhotoURI = await _imageService.SaveImage(photo, "User");
            }

            var response = await _userRepository.Update(user);

            return _mapper.Map<UserResponse>(response);
        }

        async Task<UserResponse> IUserService.GetById(string id, string companyIdClient)
        {
            var userModel = await GetById(id, companyIdClient);

            return _mapper.Map<UserResponse>(userModel);
        }

        public async Task AddWarehouseToAdmins(string warehouseId, string companyId)
        {
            var admins = await _userRepository.GetByRole(UserRole.admin, companyId);

            foreach (var admin in admins) {
                if (admin.WarehouseIds is null)
                    admin.WarehouseIds = new List<string>();

                admin.WarehouseIds.Add(warehouseId);
            }
        }

        public async Task RemoveWarehouse(string warehouseId, string companyId)
        {
            var users = await _userRepository.GetByCompanyId(companyId);

            foreach (var user in users)
                if (user.WarehouseIds is not null && user.WarehouseIds.Contains(warehouseId))
                    user.WarehouseIds.Remove(warehouseId);
        }

        public async Task<long> GetCountInWarehouse(string warehouseId) =>
            await _userRepository.GetCountInWarehouse(warehouseId);

    }
}
