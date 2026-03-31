using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.User
{
    public class UserQuery
    {
        public string? Name { get; set; }
        public string? WarehouseId { get; set; }
        public UserRole? UserRole { get; set; }

        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } 

        public int PageSize { get; set; } = 10;
        public int Page { get; set; } = 1;
    }
}
