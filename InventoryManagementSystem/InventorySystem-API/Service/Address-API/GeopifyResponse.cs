using InventorySystem_Shared.AddressClass;
using System.Text.Json.Serialization;

namespace InventorySystem_API.External_API.Adress
{
    public record class GeopifyResponse
    {
        [JsonPropertyName("features")] public List<Feature?> Features { get; set; }
    }

    public record class Feature
    {
        [JsonPropertyName("properties")] public Address Address { get; set; }
    }


}
