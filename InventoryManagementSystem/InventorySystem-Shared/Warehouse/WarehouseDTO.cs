using InventorySystem_Shared.AddressClass;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace InventorySystem_Shared.Warehouse
{
    public class WarehouseDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
        public Address Address { get; set; }
        public double Area { get; set; }

    }
}
