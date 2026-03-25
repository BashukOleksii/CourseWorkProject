using InventorySystem_API.Inventory.Service;
using InventorySystem_API.Loging.Service;
using InventorySystem_Shared.Inventory;
using InventorySystem_Shared.Loging;
using InventorySystem_Shared.User;
using Microsoft.AspNetCore.Authorization;
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
        [Audit(ActionType.ReadOne,EntityType.Inventory)]
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

        [HttpGet("warehouse/{warehoue_id}")]
        [Audit(ActionType.ReadMany, EntityType.Inventory)]
        public async Task<IActionResult> Get([FromQuery] InventoryQuery? query, string warehoue_id)
        {
            var response = await _inventoryService.Get(query, warehoue_id);
            return Ok(response);
        }

        [HttpPost("warehouse/{warehouse_Id}")]
        [Audit(ActionType.Create, EntityType.Inventory)]
        public async Task<IActionResult> Create(
            string warehouse_Id, 
            [FromForm]InventoryCreate inventoryCreate,
            IFormFile? photo)
        {
            var response = await _inventoryService.Create(inventoryCreate, warehouse_Id, photo);
            return Ok(response);
        }

        [HttpPatch("warehouse/{warehoue_id}")]
        [Audit(ActionType.Update, EntityType.Inventory)]
        public async Task<IActionResult> Update(
            string warehoue_id,
            [FromForm]InventoryUpdate inventoryUpdate,
            IFormFile? photo)
        {
            try
            {
                var response = await _inventoryService.Update(warehoue_id, inventoryUpdate, photo);
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
        [Audit(ActionType.Delete, EntityType.Inventory)]
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
