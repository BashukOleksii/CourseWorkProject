using InventorySystem_Shared.AddressClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.Warehouse
{
    public class WarehouseResponse
    {
        public string Id { get; set; } = string.Empty;
        public string CompanyId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Address Address { get; set; }
        public double Area { get; set; }
    }
}
