using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record GamePlatformReleaseCompany
{
    [JsonPropertyName("company_id")] public uint Id { get; init; }

    [JsonPropertyName("company_name")] public string Name { get; init; } = string.Empty;

    [JsonPropertyName("role")] public string Role { get; init; } = string.Empty;
}
