using InventorySystem_API.Loging.Controller;
using InventorySystem_API.User.Extention;
using InventorySystem_Shared.Loging;
using InventorySystem_Shared.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventorySystem_API.Loging.Service
{
    public class AuditActionFilter : IAsyncActionFilter
    {
        private readonly ILogService _logService;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuditActionFilter(
            ILogService logService, 
            IHttpContextAccessor contextAccessor
            )
        {
            _logService = logService;
            _contextAccessor = contextAccessor;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, 
            ActionExecutionDelegate next)
        {
            var executedContext = await next();

            var auditAttribute = context.ActionDescriptor.EndpointMetadata
                .OfType<AuditAttribute>().FirstOrDefault();

            if (auditAttribute is null)
                return;

            var user = _contextAccessor.HttpContext.User;

            string entityId = string.Empty;

            if(context.ActionArguments.ContainsKey("id"))
                entityId = context.ActionArguments["id"]?.ToString();

            if (executedContext.Result is ObjectResult executeResult)
            {

                if (executeResult is not null)
                {
                    var idProperty = executeResult.Value.GetType().GetProperty("Id");

                    if (idProperty is not null)
                        entityId = idProperty.GetValue(executeResult.Value).ToString();
                }
            }

            var auditLog = new AuditLogCreate
            {
                UserId = user.GetId(),
                UserName = user.GetName(),
                Role = Enum.Parse<UserRole>(user.GetRole()),
                UserCompanyId = user.GetCompanyId(),

                Action = auditAttribute.Action,
                EntityId = string.IsNullOrEmpty(entityId) ? "Id відсутнє" : entityId,
                EntityType = auditAttribute.Entity,
                TimeStamp = DateTime.UtcNow,
            };

            await _logService.Create(auditLog);

        }
    }
}
