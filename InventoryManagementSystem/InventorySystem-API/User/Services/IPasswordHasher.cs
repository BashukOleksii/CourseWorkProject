namespace InventorySystem_API.User.Services
{
    public interface IPasswordHasher
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string password, string passwordHash);
    }
}
