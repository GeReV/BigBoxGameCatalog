using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record Platform
{
    [JsonPropertyName("platform_id")] public uint Id { get; init; }

    [JsonPropertyName("platform_name")] public string Name { get; init; } = string.Empty;
}
