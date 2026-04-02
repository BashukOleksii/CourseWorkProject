using InventorySystem_API.User.Models;

namespace InventorySystem_API.User.Repositories
{
    public interface IPasswordResetRepository
    {
        Task Create(ResetPasswordModel resetPassword);
        Task<ResetPasswordModel?> Get(string email);
        Task Delete(string email);
    }
}
