using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace InventorySystem_Shared.AddressClass
{
    public class AddressQuery
    {
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public int? Postcode { get; set; }
    }
}
