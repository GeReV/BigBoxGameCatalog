using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class GamePlatformReleaseCompany
{
    [JsonPropertyName("company_id")] public uint Id { get; set; }

    [JsonPropertyName("company_name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("role")] public string Role { get; set; } = string.Empty;
}
