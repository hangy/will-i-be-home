namespace WillIBeHome.Owntracks
{
    using System;
    using System.Text.Json.Serialization;

    public class GetDevicesResult
    {
        [JsonPropertyName("results")]
        public string[] Results { get; set; } = Array.Empty<string>();
    }
}
