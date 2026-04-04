using AutoMapper;
using InventorySystem_API.Service.Image;
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
        private readonly IMapper _mapper;

        private readonly TokenGenerator _tokenGenerator;
        private readonly IHasher _passwordHasher;
        private readonly JWTSettingOptions _jWTSettingOptions;

        private readonly IWarehouseService _warehouseService;
        private readonly IImageService _imageService;
        

        public AuthService(
            IUserRepository userRepositorum,
            TokenGenerator tokenGenerator,
            IHasher passwordHasher,
            IMapper mapper,
            IOptions<JWTSettingOptions> options,
            IWarehouseService warehouseService,
            IImageService imageService
            )
        {
            _userRepository = userRepositorum;
            _tokenGenerator = tokenGenerator;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _jWTSettingOptions = options.Value;
            _warehouseService = warehouseService;
            _imageService = imageService;
        }

        private async Task<TokensDataResponse> GenerateTokens(UserModel user)
        {
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            user.RefeshToken = refreshToken;

            await _userRepository.Update(user);

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
            var user = await _userRepository.GetByEmail(userLogin.Email);

            if (user is null)
                throw new ArgumentException("Невірна пошта або пароль");

            if(!_passwordHasher.Verify(userLogin.Password,user!.PasswordHash))
                throw new ArgumentException("Невірна пошта або пароль");

            return await GenerateTokens(user);

        }

        public async Task<TokensDataResponse> Refresh(UserRefresh userRefresh)
        {
            var user = await _userRepository.GetById(userRefresh.UserId);

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

        public async Task<UserResponse> Register(UserRegister userRegister, IFormFile? photo)
        {
            var user = await _userRepository.GetByEmail(userRegister.Email);

            if (user is not null)
                throw new ArgumentException($"Користувач із електроною адресою:{userRegister.Email} вже є в системі");

            var userModel = _mapper.Map<UserModel>(userRegister);

            userModel.PasswordHash = _passwordHasher.Hash(userRegister.Password);

            if(userModel.UserRole == UserRole.admin)
                userModel.WarehouseIds = await _warehouseService.GetIdsByCompanyId(userModel.CompanyId);

            if (photo is null)
                userModel.PhotoURI = _imageService.GetDefaultImage("User");
            else
                userModel.PhotoURI = await _imageService.SaveImage(photo, "User");

            var response = await _userRepository.Create(userModel);

            return _mapper.Map<UserResponse>(response);
        }
    }
}
