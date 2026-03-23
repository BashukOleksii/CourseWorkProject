using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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

        public Address() { }

        public Address(
            string country,
            string state,
            string district,
            string city,
            string street,
            string housenumber,
            int postcode)
        {
            Country = country;
            State = state;
            District = district;
            City = city;
            Street = street;
            HouseNumber = housenumber;
            Postcode = postcode;
        }

        public Address(
            double latitude,
            double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public void Normalize()
        {
            if (Country is not null)
                Country = NormilizeStringInfo(Country);
            if (State is not null)
                State = NormilizeStringInfo(State, "область");
            if (District is not null)
                District = NormilizeStringInfo(District, "район");
            if (City is not null)
                City = NormilizeStringInfo(City);
            if (Street is not null)
                Street = NormilizeStringInfo(Street, "вулиця");
        }

        private string NormilizeStringInfo(string str, string? add = null)
        {
            string normal = Regex.Replace(str, @"\s+", "").ToLower();

            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            normal = textInfo.ToTitleCase(normal);

            if (add is not null && normal.Contains(add))
                normal = normal.Replace(add, "");

            return normal;
        }

    }
}
