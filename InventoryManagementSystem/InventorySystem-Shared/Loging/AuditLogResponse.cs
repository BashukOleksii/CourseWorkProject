using InventorySystem_Shared.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.Loging
{
    public class AuditLogResponse
    {
        public string Id { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public string UserCompanyId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public UserRole Role { get; set; }

        public ActionType Action { get; set; }

        public string EntityId { get; set; } = string.Empty;
        public EntityType EntityType { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
