using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace InventorySystem_Shared.AddressClass
{
    public class Address
    {
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("country")] public string? Country { get; set; }
        [JsonPropertyName("state")] public string? State { get; set; }
        [JsonPropertyName("district")] public string? District { get; set; }
        [JsonPropertyName("city")] public string? City { get; set; }
        [JsonPropertyName("street")] public string? Street { get; set; }
        [JsonPropertyName("housenumber")] public string? HouseNumber { get; set; }
        [JsonPropertyName("postcode")] public int? Postcode { get; set; }

        [JsonPropertyName("lat")] public double? Latitude { get; set; }
        [JsonPropertyName("lon")] public double? Longitude { get; set; }
    }
}
