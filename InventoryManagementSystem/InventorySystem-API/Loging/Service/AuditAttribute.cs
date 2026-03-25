using InventorySystem_Shared.Loging;

namespace InventorySystem_API.Loging.Service
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuditAttribute : Attribute
    {
        public ActionType Action;
        public EntityType Entity;

        public AuditAttribute(ActionType action, EntityType entity)
        {
            Action = action;
            Entity = entity;
        }
    }
}
