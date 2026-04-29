using InventorySystem_Shared.Company;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.Inventory
{
    public class SalesReportRequest
    {
        public string[] InventoryIds { get; set; }
        public CompanyDTO Provider { get; set; }
    }
}
