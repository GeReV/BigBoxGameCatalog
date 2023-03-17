using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record GamePlatform
{
    [JsonPropertyName("platform_id")] public uint Id { get; init; }

    [JsonPropertyName("platform_name")] public string Name { get; init; } = string.Empty;

    [JsonPropertyName("first_release_date")]
    public string FirstReleaseDate { get; init; } = string.Empty;

    [JsonPropertyName("game_id")] public uint? GameId { get; init; }

    [JsonPropertyName("attributes")] public List<Attribute> Attributes { get; init; } = new();

    // TODO: Unknown type.
    [JsonPropertyName("patches")] public List<JsonValue> Patches { get; init; } = new();

    [JsonPropertyName("ratings")] public List<Rating> Ratings { get; init; } = new();

    [JsonPropertyName("releases")] public List<GamePlatformRelease> Releases { get; init; } = new();
}
