using InventorySystem_Shared.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.Loging
{
    public class AuditLogQuery
    {
        public string? UserId { get; set; } 
        public string? UserName { get; set; } 
        public UserRole? Role { get; set; }

        public ActionType? Action { get; set; }

        public string? EntityId { get; set; } 
        public EntityType? EntityType { get; set; }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int PageSize { get; set; } = 20;
        public int Page { get; set; } = 1;
    }
}
