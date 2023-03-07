using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class GamePlatform : Platform
{
    [JsonPropertyName("first_release_date")]
    public string FirstReleaseDate { get; set; }

    [JsonPropertyName("game_id")] public uint? GameId { get; set; }

    [JsonPropertyName("attributes")] public List<Attribute> Attributes { get; set; } = new();

    [JsonPropertyName("ratings")] public List<Rating> Ratings { get; set; } = new();
}
