using InventorySystem_Shared.AddressClass;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace InventorySystem_API.External_API.Adress
{
    public class GeopifyAddressService : IAddressService
    {
        private readonly GeopifyAPIKeys _apiKeys;
        private readonly HttpClient _httpClient;

        public GeopifyAddressService(IOptions<GeopifyAPIKeys> apiKeys)
        {
            _apiKeys = apiKeys.Value;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.geoapify.com/v1/geocode/"),
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        public async Task<Address?> GetByAddress(Address address)
        {
            address.Normalize();

            try
            {
                var query =
                    $"search?country={Uri.EscapeDataString(address.Country)}" + 
                    $"&state={Uri.EscapeDataString(address.State)}" +
                    $"&city={Uri.EscapeDataString(address.City)}" +
                    $"&district={Uri.EscapeDataString(address.District)}" +
                    $"&street={Uri.EscapeDataString(address.Street)}" +
                    $"&housenumber={Uri.EscapeDataString(address.HouseNumber)}" +
                    $"&apiKey={_apiKeys.AdressAPIKey}" +
                    $"&lang=uk&limit=1";

                Console.WriteLine("HELLO");

                var reponse = await _httpClient.GetFromJsonAsync<GeopifyResponse>(query);


                return reponse.Features[0].Address;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
                
        }

        public async Task<Address?> GetByLocation(Address address)
        {

            try
            {
                var query =
                    $"reverse?lat={Convert.ToString(address.Latitude,CultureInfo.InvariantCulture)}" +
                    $"&lon={Convert.ToString(address.Longitude,CultureInfo.InvariantCulture)}" +
                    $"&apiKey={_apiKeys.LocationAPIKey}" +
                    $"&lang=uk&limit=1";


                var reponse = await _httpClient.GetFromJsonAsync<GeopifyResponse>(query);


                
                return reponse.Features[0].Address;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + _apiKeys.LocationAPIKey);
                return null;
            }
        }
    }
}
