using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.User
{
    public class UserRegister
    {
        public string CompanyId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
