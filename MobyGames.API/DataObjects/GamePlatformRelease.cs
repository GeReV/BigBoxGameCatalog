using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class GamePlatformRelease
{
    [JsonPropertyName("release_date")] public string ReleaseDate { get; set; }

    [JsonPropertyName("companies")] public List<GamePlatformReleaseCompany> Companies { get; set; } = new();

    [JsonPropertyName("countries")] public List<string> Countries { get; set; } = new();

    [JsonPropertyName("description")] public string? Description { get; set; }

    // TODO: Unknown type.
    [JsonPropertyName("product_codes")] public List<object> ProductCodes { get; set; } = new();
}
