namespace InventorySystem_API.User.Model
{
    public class RefeshToken
    {
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
    }
}
