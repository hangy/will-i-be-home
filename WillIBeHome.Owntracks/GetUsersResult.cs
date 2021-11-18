using System.Text.Json.Serialization;

namespace WillIBeHome.Owntracks;

public class GetUsersResult
{
    [JsonPropertyName("results")]
    public string[] Results { get; set; } = Array.Empty<string>();
}
