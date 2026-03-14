using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.Warehouse
{
    public class WarehouseResponse : WarehouseDTO
    {
        public string Id { get; set; } = string.Empty;
        public string CompanyId { get; set; } = string.Empty;
    }
}
