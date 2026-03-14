using AutoMapper;
using InventorySystem_API.User.Model;
using InventorySystem_API.User.Repositories;
using InventorySystem_API.Warehouse.Service;
using InventorySystem_Shared.User;
using Microsoft.Extensions.Options;

namespace InventorySystem_API.User.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly TokenGenerator _tokenGenerator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly JWTSettingOptions _jWTSettingOptions;
        private readonly IWarehouseService _warehouseService;

        public AuthService(
            IUserRepository userRepositorum,
            TokenGenerator tokenGenerator,
            IPasswordHasher passwordHasher,
            IMapper mapper,
            IOptions<JWTSettingOptions> options,
            IWarehouseService warehouseService
            )
        {
            _userRepository = userRepositorum;
            _tokenGenerator = tokenGenerator;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _jWTSettingOptions = options.Value;
            _warehouseService = warehouseService;
        }

        private async Task<TokensDataResponse> GenerateTokens(UserModel user)
        {
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            user.RefeshToken = refreshToken;

            await _userRepository.UpdateAsync(user);

            return new TokensDataResponse
            {
                AccessToken = _tokenGenerator.GenerateAccessToken(user),
                AccessTokenExpiresMinuts = _jWTSettingOptions.ExpiryInMinutes,
                RefreshToken = refreshToken.RefreshToken,
                RefreshTokenExpireDays = _jWTSettingOptions.RefreshTokenExpiryInDays
            };
        }

        public async Task<TokensDataResponse> LogIn(UserLogin userLogin)
        {
            var user = await _userRepository.GetUserByEmailAsync(userLogin.Email);

            if (user is null)
                new ArgumentException("Невірна пошта або пароль");

            if(!_passwordHasher.VerifyPassword(userLogin.Password,user!.PasswordHash))
                new ArgumentException("Невірна пошта або пароль");

            return await GenerateTokens(user);

        }

        public async Task<TokensDataResponse> Refresh(UserRefresh userRefresh)
        {
            var user = await _userRepository.GetUserByIdAsync(userRefresh.UserId);

            if (user is null)
                throw new KeyNotFoundException($"Користувача із id:{userRefresh.UserId} не знайдено");

            if (
                user.RefeshToken is null ||
                user.RefeshToken.RefreshToken != userRefresh.RefreshToken ||
                user.RefeshToken.ExpiryDate < DateTime.UtcNow
            )
                throw new ArgumentException("Невірний Refresh token");

            return await GenerateTokens(user);
        }

        public async Task<UserResponse> Register(UserRegister userRegister)
        {
            var user = await _userRepository.GetUserByEmailAsync(userRegister.Email);

            if (user is not null)
                throw new ArgumentException($"Користувач із електроною адресою:{userRegister.Email} вже є в системі");

            var userModel = _mapper.Map<UserModel>(userRegister);

            if(userModel.UserRole == UserRole.admin)
                userModel.WarehouseIds = await _warehouseService.GetIdsByCompanyId(userModel.CompanyId);
            

            var response = await _userRepository.CreateAsync(userModel);

            return _mapper.Map<UserResponse>(response);
        }
    }
}
