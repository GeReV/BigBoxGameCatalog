using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record Group
{
    [JsonPropertyName("group_id")] public uint Id { get; init; }

    [JsonPropertyName("group_name")] public string Name { get; init; } = string.Empty;

    [JsonPropertyName("group_description")]
    public string Description { get; init; } = string.Empty;
}
