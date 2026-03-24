using InventorySystem_API.Inventory.Service;
using InventorySystem_Shared.Inventory;
using InventorySystem_Shared.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_API.Inventory.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(UserRole.manager))]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService) =>
            _inventoryService = inventoryService;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var response = await _inventoryService.GetById(id);
                return Ok(response);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("warehouse/{id}")]
        public async Task<IActionResult> Get([FromQuery] InventoryQuery? query, string id)
        {
            var response = await _inventoryService.Get(query, id);
            return Ok(response);
        }

        [HttpPost("warehouse/{id}")]
        public async Task<IActionResult> Create(
            string id, 
            [FromForm]InventoryCreate inventoryCreate,
            IFormFile? photo)
        {
            var response = await _inventoryService.Create(inventoryCreate,id,photo);
            return Ok(response);
        }

        [HttpPatch("warehouse/{id}")]
        public async Task<IActionResult> Update(
            string id,
            [FromForm]InventoryUpdate inventoryUpdate,
            IFormFile? photo)
        {
            try
            {
                var response = await _inventoryService.Update(id, inventoryUpdate, photo);
                return Ok(response);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("id")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _inventoryService.DeleteById(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
