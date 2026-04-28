using InventorySystem_MAUI.Helper.Exceptions;
using InventorySystem_Shared.AddressClass;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace InventorySystem_MAUI.Service
{
    public interface IAddressService
    {
        Task<Address> GetByAddress(Address address, string route);
    }

}
