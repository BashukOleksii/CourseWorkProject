using InventorySystem_API.Report.Service;
using InventorySystem_API.User.Extention;
using InventorySystem_Shared.Inventory;
using InventorySystem_Shared.User;
using InventorySystem_Shared.Warehouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_API.Report.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ExportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [Authorize(Roles = nameof(UserRole.manager))]
        [HttpGet("inventory-report")]
        public async Task<IActionResult> GetInventoryReport([FromQuery] InventoryQuery? inventoryQuery, string warehouseId)
        {
            try
            {
                var pdfBytes = await _reportService.GetInventoryReport(inventoryQuery, warehouseId);

                var fileName = $"InventoryReport_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch(InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = nameof(UserRole.manager))]
        [HttpGet("sales-report")]
        public async Task<IActionResult> GetSalesReport([FromQuery] string[] InventoryIds)
        {
            try
            {
                var pdfBytes = "";// = await _reportService.GetSalesReport(salesQuery, User.GetCompanyId());
                var fileName = $"SalesReport_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [Authorize(Roles = nameof(UserRole.admin))]
        [HttpGet("warehouse-report")]
        public async Task<IActionResult> GetWarehouseReport([FromQuery] WarehouseQuery? warehouseQuery)
        {
            try
            {
                var pdfBytes = await _reportService.GetWarehouseReport(warehouseQuery, User.GetCompanyId());

                var fileName = $"WarehouseReport_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [Authorize(Roles = nameof(UserRole.admin))]
        [HttpGet("user-report")]
        public async Task<IActionResult> GetUserReport([FromQuery] UserQuery? userQuery)
        {
            try
            {
                var pdfBytes = await _reportService.GetUserReport(userQuery, User.GetCompanyId());

                var fileName = $"UserReport_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }


    }
}
