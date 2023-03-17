using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record Attribute
{
    [JsonPropertyName("attribute_id")] public uint Id { get; init; }

    [JsonPropertyName("attribute_name")] public string Name { get; init; } = string.Empty;

    [JsonPropertyName("attribute_category_id")]
    public uint AttributeCategoryId { get; init; }

    [JsonPropertyName("attribute_category_name")]
    public string AttributeCategoryName { get; init; } = string.Empty;
}
