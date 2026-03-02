using InventorySystem_API.User.Services;
using InventorySystem_Shared.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_API.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) =>
            _authService = authService;

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(UserRegister userRegister)
        {
            try
            {
                var response = await _authService.Register(userRegister);
                return Ok(response);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(UserLogin userLogin)
        {
            try
            {
                var response = await _authService.LogIn(userLogin);
                return Ok(response);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(UserRefresh userRefresh)
        {
            try
            {
                var response = await _authService.Refresh(userRefresh);
                return Ok(response);
            }
            catch (KeyNotFoundException kex)
            {
                return NotFound(kex.Message);
            }
            catch (ArgumentException aex)
            {
                return Unauthorized(aex.Message);
            }
        }
        
    }
}
