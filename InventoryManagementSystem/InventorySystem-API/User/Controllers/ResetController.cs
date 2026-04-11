using InventorySystem_API.User.Services;
using InventorySystem_Shared.User;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_API.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetController : ControllerBase
    {
        public readonly IPasswordResetService _passwordResetService;

        public ResetController(IPasswordResetService passwordResetService)
        {
            _passwordResetService = passwordResetService;
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword([FromQuery] string email)
        {
            try
            {
                await _passwordResetService.GenerateResetCode(email);
                return NoContent();

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromQuery] string code, ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                await _passwordResetService.CheckCode(resetPasswordDTO.Email, code);
                await _passwordResetService.ChangePassword(resetPasswordDTO.Email, resetPasswordDTO.Password);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
