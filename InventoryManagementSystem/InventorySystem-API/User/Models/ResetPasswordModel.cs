using MongoDB.Bson.Serialization.IdGenerators;

namespace InventorySystem_API.User.Models
{
    public class ResetPasswordModel
    {
        public string Email { get; set; } = string.Empty;
        public string CodeHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
