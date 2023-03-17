using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record Genre
{
    [JsonPropertyName("genre_id")] public uint Id { get; init; }

    [JsonPropertyName("genre_name")] public string Name { get; init; } = string.Empty;

    [JsonPropertyName("genre_description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("genre_category_id")]
    public uint CategoryId { get; init; }

    [JsonPropertyName("genre_category")] public string Category { get; init; } = string.Empty;
}
