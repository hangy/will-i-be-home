using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WillIBeHome.Owntracks
{
    public class Location
    {
        [JsonPropertyName("_type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("acc")]
        public int? Accuracy { get; set; }

        [JsonPropertyName("alt")]
        public int? Altitude { get; set; }

        [JsonPropertyName("batt")]
        public int? Battery { get; set; }

        [JsonPropertyName("bs")]
        public BatteryStatus BatteryStatus { get; set; }

        [JsonPropertyName("cog")]
        public int? CourseOverGround { get; set; }

        [JsonPropertyName("conn")]
        public string Connection { get; set; } = string.Empty;

        [JsonPropertyName("inregions")]
        public IReadOnlyCollection<string> InRegions { get; set; } = new List<string>();

        [JsonPropertyName("lat")]
        public float Latitude { get; set; }

        [JsonPropertyName("lon")]
        public float Longitude { get; set; }

        [JsonPropertyName("rad")]
        public int? Radius { get; set; }

        [JsonPropertyName("t")]
        public string Trigger { get; set; } = string.Empty;

        [JsonPropertyName("tid")]
        public string? TrackerId { get; set; }

        [JsonPropertyName("tst")]
        public int? UnixEpochTimestamp { get; set; }

        [JsonPropertyName("vac")]
        public int? VerticalAccuracy { get; set; }

        [JsonPropertyName("vel")]
        public int? Velocity { get; set; }

        [JsonPropertyName("p")]
        public float? BarometicPressure { get; set; }

        [JsonPropertyName("ghash")]
        public string? Geohash { get; set; }

        [JsonPropertyName("cc")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("addr")]
        public string? Address { get; set; }

        [JsonPropertyName("locality")]
        public string? Locality { get; set; }
    }
}
