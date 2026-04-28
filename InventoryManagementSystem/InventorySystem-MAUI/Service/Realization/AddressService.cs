using InventorySystem_MAUI.Helper.Exceptions;
using InventorySystem_Shared.AddressClass;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace InventorySystem_MAUI.Service
{
    public class AddressService
    {
        private readonly HttpClient _httpClient;

        public AddressService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIClient");
        }

        public async Task<Address> GetByAddress(Address address, string route)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/address/{route}", address);

            return await response.Content.ReadFromJsonAsync<Address>();

        }

    }

}
