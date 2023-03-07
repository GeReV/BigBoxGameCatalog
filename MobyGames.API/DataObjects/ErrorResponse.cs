using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class MobyGamesErrorResponse
{
    [JsonPropertyName("code")] public uint Code { get; set; }

    [JsonPropertyName("error")] public string? Error { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }
}
