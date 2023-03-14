using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class Platform
{
    [JsonPropertyName("platform_id")] public uint Id { get; set; }

    [JsonPropertyName("platform_name")] public string Name { get; set; } = string.Empty;
}
