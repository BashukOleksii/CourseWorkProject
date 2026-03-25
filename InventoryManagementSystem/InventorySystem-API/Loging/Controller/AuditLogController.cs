using InventorySystem_API.Loging.Service;
using InventorySystem_API.User.Extention;
using InventorySystem_Shared.Loging;
using InventorySystem_Shared.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_API.Loging.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(UserRole.admin))]
    public class AuditLogController : ControllerBase
    {
        private readonly ILogService _logService;

        public AuditLogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AuditLogQuery query)
        {
            try
            {
                var response = await _logService.Get(query, User.GetCompanyId());
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
