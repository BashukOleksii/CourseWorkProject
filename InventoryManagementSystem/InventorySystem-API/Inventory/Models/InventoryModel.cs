using InventorySystem_Shared.Inventory.Manufacturer;
using MongoDB.Bson.Serialization.Attributes;

namespace InventorySystem_API.Inventory.Models
{
    public class InventoryModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string WarehouseId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public InventoryManufacturer Manufacturer { get; set; }
        public InventoryType InventoryType { get; set; }


        public double Price { get; set; }
        public int Quantity { get; set; }
        public Dictionary<string, string>? CustomFileds { get; set; }

        public string PhotoURI { get; set; } = string.Empty;

    }
}
