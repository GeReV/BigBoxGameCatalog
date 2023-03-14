using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public record Genre
{
    [JsonPropertyName("genre_id")] public uint Id { get; set; }

    [JsonPropertyName("genre_name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("genre_description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("genre_category_id")]
    public uint CategoryId { get; set; }

    [JsonPropertyName("genre_category")] public string Category { get; set; } = string.Empty;
}
