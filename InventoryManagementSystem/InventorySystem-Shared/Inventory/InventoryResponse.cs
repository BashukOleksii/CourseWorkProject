using InventorySystem_API.Inventory.Models;
using InventorySystem_Shared.Inventory.Manufacturer;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.Inventory
{
    public class InventoryResponse
    {
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
