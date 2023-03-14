using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class GamePlatform
{
    [JsonPropertyName("platform_id")] public uint Id { get; set; }

    [JsonPropertyName("platform_name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("first_release_date")]
    public string FirstReleaseDate { get; set; } = string.Empty;

    [JsonPropertyName("game_id")] public uint? GameId { get; set; }

    [JsonPropertyName("attributes")] public List<Attribute> Attributes { get; set; } = new();

    // TODO: Unknown type.
    [JsonPropertyName("patches")] public List<JsonValue> Patches { get; set; } = new();

    [JsonPropertyName("ratings")] public List<Rating> Ratings { get; set; } = new();

    [JsonPropertyName("releases")] public List<GamePlatformRelease> Releases { get; set; } = new();
}
