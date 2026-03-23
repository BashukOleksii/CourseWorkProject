using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.User
{
    internal class UserQuery
    {
        public string? Name { get; set; }
        public string? CompanyId { get; set; }
        public string? WarehouseId { get; set; }
        public UserRole? UserRole { get; set; }

        public int PageSie { get; set; } = 10;
        public int Page { get; set; } = 1;
    }
}
