using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record CoverGroup
{
    [JsonPropertyName("comments")] public string? Comments { get; init; }

    [JsonPropertyName("countries")] public List<string> Countries { get; init; } = new();

    [JsonPropertyName("covers")] public List<Cover> Covers { get; init; } = new();
}
