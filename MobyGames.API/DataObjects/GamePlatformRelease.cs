using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record GamePlatformRelease
{
    [JsonPropertyName("release_date")] public string ReleaseDate { get; init; } = string.Empty;

    [JsonPropertyName("companies")] public List<GamePlatformReleaseCompany> Companies { get; init; } = new();

    [JsonPropertyName("countries")] public List<string> Countries { get; init; } = new();

    [JsonPropertyName("description")] public string? Description { get; init; }

    [JsonPropertyName("product_codes")] public List<GameReleaseProductCode> ProductCodes { get; init; } = new();
}
