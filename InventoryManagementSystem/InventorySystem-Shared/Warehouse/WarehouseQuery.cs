using InventorySystem_Shared.AddressClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.Warehouse
{
    public class WarehouseQuery
    {
        public string? Name { get; set; } 
        public string? PartDescription { get; set; }
        public AddressQuery? Address { get; set; }
        
        public double? MinArea { get; set; }
        public double? MaxArea { get; set; }

        public int PageSize { get; set; } = 5;
        public int Page { get; set; } = 1;
    }
}
