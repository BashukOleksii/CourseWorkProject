using InventorySystem_API.User.Extention;
using InventorySystem_API.User.Services;
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
        [Authorize(Roles = nameof(UserRole.admin))]
        public async Task<IActionResult> GetUserById(string id)
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
        [Authorize(Roles = nameof(UserRole.admin))]
        public async Task<IActionResult> GetUsersByCompany()
        {
            var user = await _userService.Get(User.GetCompanyId());
            return Ok(user);
        }

        [Authorize(Roles = nameof(UserRole.admin))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
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
        [HttpPost("{id}/warehouses")]
        public async Task<IActionResult> AddWarehouses(string id, string[] warehouses)
        {
            try
            {
                await _userService.AddWarehouses(id, warehouses, User.GetCompanyId());
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
        [HttpDelete("{id}/warehouses")]
        public async Task<IActionResult> DeleteWarehouses(string id, string[] warehouses)
        {
            try
            {
                await _userService.RemoveWarehouses(id, warehouses, User.GetCompanyId());
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, UserUpdate userUpdate)
        {
            try
            {
                var updated = await _userService.Update(id, userUpdate, User.GetCompanyId());
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
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetById(id!, User.GetCompanyId());
            return Ok(user);
        }

    }
}
