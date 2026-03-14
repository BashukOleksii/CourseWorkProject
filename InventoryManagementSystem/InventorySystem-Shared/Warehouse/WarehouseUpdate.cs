using InventorySystem_Shared.AddressClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.Warehouse
{
    public class WarehouseUpdate
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Address? Address { get; set; }
        public double? Area { get; set; }
    }
}
