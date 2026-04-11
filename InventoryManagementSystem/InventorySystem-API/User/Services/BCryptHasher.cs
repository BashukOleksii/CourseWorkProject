namespace InventorySystem_API.User.Services
{
    public class BCryptHasher : IHasher
    {
        public string Hash(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string password, string passwordHash) =>
            BCrypt.Net.BCrypt.Verify(password, passwordHash);
        
    }
}
