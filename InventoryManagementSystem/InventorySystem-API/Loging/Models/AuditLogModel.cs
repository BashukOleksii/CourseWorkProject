using InventorySystem_Shared.Loging;
using InventorySystem_Shared.User;
using MongoDB.Bson.Serialization.Attributes;

namespace InventorySystem_API.Loging.Models
{
    public class AuditLogModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
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
