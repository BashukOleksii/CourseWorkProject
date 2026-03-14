using InventorySystem_Shared.AddressClass;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InventorySystem_API.Warehouse.Models
{
    public class WarehouseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string CompanyId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Address Address { get; set; }
        public double Area { get; set; }
    }
}
