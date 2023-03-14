using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class GamePlatformRelease
{
    [JsonPropertyName("release_date")] public string ReleaseDate { get; set; } = string.Empty;

    [JsonPropertyName("companies")] public List<GamePlatformReleaseCompany> Companies { get; set; } = new();

    [JsonPropertyName("countries")] public List<string> Countries { get; set; } = new();

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("product_codes")] public List<GameReleaseProductCode> ProductCodes { get; set; } = new();
}
