using AutoMapper;
using InventorySystem_API.User.Model;
using InventorySystem_API.User.Repositories;
using InventorySystem_API.User.Validator;
using InventorySystem_Shared.User;
using System.Text.RegularExpressions;

namespace InventorySystem_API.User.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserModelValidator _validationRules;
        private readonly BCryptPasswordHasher _passwordHasher;
        
        public UserService(
            IUserRepository userRepository, 
            IMapper mapper,
            UserModelValidator validationRules,
            BCryptPasswordHasher bCryptPasswordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _validationRules = validationRules;
            _passwordHasher = bCryptPasswordHasher;
        }

        private async Task<UserModel> GetUserById(string userId, string companyIdClient)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user is null)
                throw new KeyNotFoundException($"Користувача із {userId} не знайдено");

            if (user!.CompanyId != companyIdClient)
                throw new ArgumentException("Це не працівник вашої фірми");

            return user;
        }

        public async Task AddWarehouses(string userId, string[] warehouses, string companyIdClient)
        {
            var user = await GetUserById(userId, companyIdClient);

            if(user.WarehouseIds is null)
                user.WarehouseIds = new List<string>();

            user.WarehouseIds.AddRange(warehouses);

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUser(string id, string companyIdClient)
        {
            var user = await GetUserById(id, companyIdClient);

            await _userRepository.DeleteAsync(user.Id);
        }

        public async Task<List<UserResponse>> GetUsers(string companyIdClient)
        {
            var users = await _userRepository.GetUsersByCompanyId(companyIdClient);

           return _mapper.Map<List<UserResponse>>(users);
        }


        public async Task RemoveWarehouses(string userId, string[] warehouses, string companyIdClient)
        {
            var user = await GetUserById(userId, companyIdClient);

            foreach(var warehouse in warehouses)
            {
                if (user.WarehouseIds is null || !user.WarehouseIds.Contains(warehouse))
                    throw new ArgumentException($"Працівник не має складу із id:{warehouse}"); 

                user.WarehouseIds.Remove(warehouse);
            }

            await _userRepository.UpdateAsync(user);

        }

        public async Task<UserResponse> UpdateUser(string userId, UserUpdate userUpdate, string companyIdClient)
        {
            var user = await GetUserById(userId, companyIdClient);

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

            var response = await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserResponse>(response);
        }

        async Task<UserResponse> IUserService.GetUserById(string id, string companyIdClient)
        {
            var userModel = await GetUserById(id, companyIdClient);

            return _mapper.Map<UserResponse>(userModel);
        }
     

    }
}
