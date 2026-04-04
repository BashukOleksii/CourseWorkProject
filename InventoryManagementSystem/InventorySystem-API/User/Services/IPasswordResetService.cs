namespace InventorySystem_API.User.Services
{
    public interface IPasswordResetService
    {
        Task GenerateResetCode(string email);
        Task CheckCode(string email, string code);
        Task ChangePassword(string email, string newPassword);
    }
}
