using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record MobyGamesErrorResponse
{
    [JsonPropertyName("code")] public uint Code { get; init; }

    [JsonPropertyName("error")] public string? Error { get; init; }

    [JsonPropertyName("message")] public string? Message { get; init; }
}
