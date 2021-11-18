using System.Text.Json.Serialization;

namespace WillIBeHome.Owntracks;

public class GetLocationsResult
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("data")]
    public IReadOnlyList<Location> Data { get; set; } = new List<Location>();
}
