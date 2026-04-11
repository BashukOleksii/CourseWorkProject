using InventorySystem_Shared.AddressClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.Company
{
    public class CompanyResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Address Address { get; set; }
        public string Phone { get; set; }
    }
}
