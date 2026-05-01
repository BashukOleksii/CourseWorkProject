using System.Net;
using System.Net.Http.Json;
using InventorySystem_Shared.AddressClass;

namespace InventoryManagementSystem_Tests.Integration.Controller
{
    public class AddressIntegrationTests : BaseTest
    {
        public AddressIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task GetByAddress_ValidAddress_ReturnsOk()
        {
            var addressRequest = new Address
            {
                Country = "Укрїна",
                State = "Хмельницька",
                District = "Хмельницький",
                City = "Хмельницький",
                Street = "Зарічанська",
                HouseNumber = "10/4"
            };

            var response = await Client.PostAsJsonAsync("api/address/address", addressRequest);

            var result = await response.Content.ReadFromJsonAsync<Address>();
            Assert.NotNull(result);
            Assert.Equal("Україна", result.Country);

        }

        [Fact]
        public async Task GetByAddress_EmptyAddress_ReturnsNotFound()
        {
            var addressRequest = new Address();

            var response = await Client.PostAsJsonAsync("api/address/address", addressRequest);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var message = await response.Content.ReadAsStringAsync();
            Assert.Contains("Адреси не знайдено", message);
        }

        [Fact]
        public async Task GetByLocation_ValidCoordinates_ReturnsOk()
        {
            var addressRequest = new Address
            {
                Latitude = 50.4501,
                Longitude = 30.5234
            };

            var response = await Client.PostAsJsonAsync("api/address/location", addressRequest);

            var result = await response.Content.ReadFromJsonAsync<Address>();
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.City));

        }

     

    }
}
