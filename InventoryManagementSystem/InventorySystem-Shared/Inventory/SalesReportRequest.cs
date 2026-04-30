using InventorySystem_Shared.Company;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.Inventory
{
    public class SalesReportRequest
    {
        public InventoryInfo[] InventoryInfo { get; set; }
        public CompanyDTO Provider { get; set; }
    }

    public class InventoryInfo
    {
        public string InventoryId { get; set; }
        public int Quantity { get; set; }
    }
}
