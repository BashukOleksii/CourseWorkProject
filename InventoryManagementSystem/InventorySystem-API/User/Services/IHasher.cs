namespace InventorySystem_API.User.Services
{
    public interface IHasher
    {
        public string Hash(string password);
        public bool Verify(string password, string passwordHash);
    }
}
