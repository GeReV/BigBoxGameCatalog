using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record AlternateTitle
{
    [JsonPropertyName("title")] public string Title { get; init; } = string.Empty;

    [JsonPropertyName("description")] public string Description { get; init; } = string.Empty;
}
