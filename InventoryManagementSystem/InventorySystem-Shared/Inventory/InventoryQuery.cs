using InventorySystem_API.Inventory.Models;
using InventorySystem_Shared.Inventory.Manufacturer;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.Inventory
{
    public class InventoryQuery
    {

        public string? Name { get; set; } 
        public string? Description { get; set; } 
        public InventoryManufacturerDTO? Manufacturer { get; set; }
        public InventoryType? InventoryType { get; set; }


        public double? MinPrice { get; set; }
        public double? MsxPrice { get; set; }

        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }

        public int PageSize { get; set; } = 15;
        public int Page { get; set; } = 1;

    }
}
