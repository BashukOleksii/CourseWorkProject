    using InventorySystem_API.Loging.Service;
    using InventorySystem_API.User.Extention;
    using InventorySystem_API.User.Services;
    using InventorySystem_Shared.Loging;
    using InventorySystem_Shared.User;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    namespace InventorySystem_API.User.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        [Authorize]
        public class UserController : ControllerBase
        {
            private readonly IUserService _userService;

            public UserController(IUserService userService)
            {
                _userService = userService;
            }

            [HttpGet("{id}")]
            [Audit(ActionType.ReadOne, EntityType.User)]
            [Authorize(Roles = nameof(UserRole.admin))]
            public async Task<IActionResult> GetById(string id)
            {
                try
                {
                    var user = await _userService.GetById(id, User.GetCompanyId());
                    return Ok(user);
                }
                catch(KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            [HttpGet]
            [Audit(ActionType.ReadMany, EntityType.User)]
            [Authorize(Roles = nameof(UserRole.admin))]
            public async Task<IActionResult> Get([FromQuery] UserQuery userQuery)
            {
                try
                {
                    var user = await _userService.Get(User.GetCompanyId(), userQuery, User.GetId());
                    return Ok(user);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            [Authorize(Roles = nameof(UserRole.admin))]
            [HttpDelete("{id}")]
            [Audit(ActionType.Delete, EntityType.User)]
            public async Task<IActionResult> Delete(string id)
            {
                try
                {
                    await _userService.Delete(id, User.GetCompanyId());
                    return NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            [Authorize(Roles = nameof(UserRole.admin))]
            [HttpPatch("{id}/warehouses")]
            [Audit(ActionType.Update, EntityType.User)]
            public async Task<IActionResult> AddWarehouses(string id, [FromBody]string[] warehouses)
            {
                try
                {
                    await _userService.UpdateWarehouses(id, warehouses, User.GetCompanyId());
                    return NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            [Authorize(Roles = nameof(UserRole.admin))]

            [HttpPatch("{id}")]
            [Audit(ActionType.Update, EntityType.User)]
            public async Task<IActionResult> Update(
                string id,
                [FromForm] UserUpdate userUpdate,
                IFormFile? photo
                )
            {
                try
                {
                    var updated = await _userService.Update(id, userUpdate, User.GetCompanyId(),photo);
                    return Ok(updated);
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            [HttpGet("whoami")]
            public async Task<IActionResult> GetCurentUser()
            {
                var id = User.GetId();
                var user = await _userService.GetById(id!, User.GetCompanyId());
                return Ok(user);
            }

        }
    }
