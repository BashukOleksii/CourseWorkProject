using InventorySystem_Shared.AddressClass;
using MongoDB.Bson.Serialization.Attributes;

namespace InventorySystem_API.Company.Models
{
    public class CompanyModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Address Address { get; set; }
        public string Phone { get; set; } = string.Empty;
    }
}
