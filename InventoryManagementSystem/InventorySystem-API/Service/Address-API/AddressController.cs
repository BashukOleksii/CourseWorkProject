using InventorySystem_API.External_API.Adress;
using InventorySystem_Shared.AddressClass;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_API.Service.Address_API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpPost("address")]
        public async Task<IActionResult> GetByAddress([FromBody] Address address)
        {
            var result = await _addressService.GetByAddress(address);
            if (result != null)
                return Ok(result);
            else
                return NotFound("Адреси не знайдено");
        }

        [HttpPost("location")]
        public async Task<IActionResult> GetByLocation([FromBody] Address address)
        {
            var result = await _addressService.GetByLocation(address);
            if (result != null)
                return Ok(result);
            else
                return NotFound("Адреси не знайдено");
        }

    }
}
