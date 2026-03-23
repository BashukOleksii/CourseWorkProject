using InventorySystem_API.Company.Service;
using InventorySystem_API.User.Extention;
using InventorySystem_Shared.Company;
using InventorySystem_Shared.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_API.Company.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCompany()
        {
            try
            {
                var company = await _companyService.GetById(User.GetCompanyId());
                return Ok(company);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCompany(CompanyDTO companyDTO)
        {
            var companyResponse = await _companyService.Create(companyDTO);
            return Ok(companyResponse);
        }

        [HttpPatch]
        [Authorize(Roles = nameof(UserRole.admin))]
        public async Task<IActionResult> UpdateMyCompany(CompanyUpdate companyUpdate)
        {
            try
            {
                var companyResponse = await _companyService.Update(User.GetCompanyId(), companyUpdate);
                return Ok(companyResponse);
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

        [HttpDelete]
        [Authorize(Roles = nameof(UserRole.admin))]
        public async Task<IActionResult> DeleteMyCompany()
        {
            try
            {
                await _companyService.Delete(User.GetCompanyId());
                return NoContent();
            }
            catch(KeyNotFoundException ex) 
            {
                return NotFound(ex.Message);
            }
        }
    }
}
