using InventorySystem_API.Inventory.Models;
using InventorySystem_Shared.Inventory.Manufacturer;

namespace InventorySystem_Shared.Inventory
{
    public class    InventoryUpdate
    {
        public string? Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public InventoryType? InventoryType { get; set; }
        public InventoryManufacturerDTO? Manufacturer { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public Dictionary<string, string>? CustomFileds { get; set; }
    }
}
