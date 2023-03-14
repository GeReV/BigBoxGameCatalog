using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class Group
{
    [JsonPropertyName("group_id")] public uint Id { get; set; }

    [JsonPropertyName("group_name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("group_description")]
    public string Description { get; set; } = string.Empty;
}
