namespace WillIBeHome.Owntracks
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class GetLocationsResult
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("data")]
        public IReadOnlyList<Location> Data { get; set; } = new List<Location>();
    }
}
