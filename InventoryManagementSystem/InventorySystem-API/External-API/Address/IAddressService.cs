
using InventorySystem_Shared.AddressClass;

namespace InventorySystem_API.External_API.Adress
{
    public interface IAddressService
    {
        Task<Address?> GetByAddress(Address address);
        Task<Address?> GetByLocation(Address address);
    }
}
