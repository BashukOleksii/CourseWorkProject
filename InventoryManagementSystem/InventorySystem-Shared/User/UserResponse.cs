using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.User
{
    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CompanyId { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhotoURI { get; set; } = string.Empty;
    }
}
