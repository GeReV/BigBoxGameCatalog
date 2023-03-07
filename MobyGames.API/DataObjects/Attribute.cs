using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class Attribute
{
    [JsonPropertyName("attribute_id")] public uint Id { get; set; }

    [JsonPropertyName("attribute_name")] public string Name { get; set; }

    [JsonPropertyName("attribute_category_id")]
    public uint AttributeCategoryId { get; set; }

    [JsonPropertyName("attribute_category_name")]
    public string AttributeCategoryName { get; set; }
}
