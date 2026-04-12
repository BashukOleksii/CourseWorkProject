using InventorySystem_API.Loging.Service;
using InventorySystem_API.User.Extention;
using InventorySystem_API.Warehouse.Service;
using InventorySystem_Shared.Loging;
using InventorySystem_Shared.User;
using InventorySystem_Shared.Warehouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_API.Warehouse.Conroller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet("{id}")]
        [Audit(ActionType.ReadOne,EntityType.Warehouse)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var companyId = User.GetCompanyId();
                var warehouse = await _warehouseService.GetById(id,companyId);
                return Ok(warehouse);
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
        [Audit(ActionType.ReadMany, EntityType.Warehouse)]
        public async Task<IActionResult> GetByIds([FromQuery] string[] ids)
        {
            try
            {
                var companyId = User.GetCompanyId();
                var warehouses = await _warehouseService.GetByIds(ids, companyId);
                return Ok(warehouses);
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

        [HttpGet]
        [Audit(ActionType.ReadMany, EntityType.Warehouse)]
        [Authorize(Roles = nameof(UserRole.admin))]
        public async Task<IActionResult> GetWarerhouses([FromQuery] WarehouseQuery? warehouseQuery)
        {
            try
            {
                var companyId = User.GetCompanyId();
                var warehouses = await _warehouseService.Get(companyId, warehouseQuery);
                return Ok(warehouses);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Audit(ActionType.Create, EntityType.Warehouse)]
        [Authorize(Roles = nameof(UserRole.admin))]
        public async Task<IActionResult> Create(WarehouseDTO dto)
        {
            var comapnyId = User.GetCompanyId();
            var created = await _warehouseService.Create(dto, comapnyId);
            return Ok(created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(UserRole.admin))]
        [Audit(ActionType.Update, EntityType.Warehouse)]
        public async Task<IActionResult> Update(string id, WarehouseUpdate update)
        {
            try
            {
                var companyId = User.GetCompanyId();
                var updated = await _warehouseService.Update(id, update, companyId);
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

        [HttpDelete("{id}")]
        [Audit(ActionType.Delete, EntityType.Warehouse)]
        [Authorize(Roles = nameof(UserRole.admin))]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var companyId = User.GetCompanyId();
                await _warehouseService.Delete(id, companyId);
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

    }
}
