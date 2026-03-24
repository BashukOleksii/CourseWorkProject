using InventorySystem_Shared.User;

namespace InventorySystem_API.User.Services
{
    public interface IAuthService
    {
        Task<UserResponse> Register(UserRegister userRegister, IFormFile? photo);
        Task<TokensDataResponse> LogIn(UserLogin userLogin);
        Task<TokensDataResponse> Refresh(UserRefresh userRefresh);

    }
}
