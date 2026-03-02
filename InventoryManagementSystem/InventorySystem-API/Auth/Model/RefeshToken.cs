namespace InventorySystem_API.Auth.Model
{
    public class RefeshToken
    {
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
    }
}
