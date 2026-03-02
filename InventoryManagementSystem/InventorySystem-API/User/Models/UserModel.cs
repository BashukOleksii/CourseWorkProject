using InventorySystem_Shared.User;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;


namespace InventorySystem_API.User.Model
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhotoURI { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string CompanyId { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public List<string>? WarehouseIds { get; set; }

        public RefeshToken? RefeshToken { get; set; }
    
       
    }
}
