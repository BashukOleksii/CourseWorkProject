using InventorySystem_API.User.Extention;
using InventorySystem_API.Warehouse.Service;
using InventorySystem_Shared.User;
using InventorySystem_Shared.Warehouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        [Authorize(Roles = nameof(UserRole.admin))]
        public async Task<IActionResult> GetCompanyWarerhouuses()
        {
            var companyId = User.GetCompanyId();
            var warehouses = await _warehouseService.GetIdsByCompanyId(companyId);
            return Ok(warehouses);
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.admin))]
        public async Task<IActionResult> Create(WarehouseDTO dto)
        {
            var comapnyId = User.GetCompanyId();
            var created = await _warehouseService.Create(dto, comapnyId);
            return Ok(created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(UserRole.admin))]
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
