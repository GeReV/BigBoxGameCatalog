using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record Rating
{
    [JsonPropertyName("rating_id")] public uint Id { get; init; }

    [JsonPropertyName("rating_name")] public string Name { get; init; } = string.Empty;

    [JsonPropertyName("rating_system_id")] public uint RatingSystemId { get; init; }

    [JsonPropertyName("rating_system_name")]
    public string RatingSystemName { get; init; } = string.Empty;
}
