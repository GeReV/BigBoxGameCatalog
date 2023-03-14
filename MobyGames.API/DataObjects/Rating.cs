using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class Rating
{
    [JsonPropertyName("rating_id")] public uint Id { get; set; }

    [JsonPropertyName("rating_name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("rating_system_id")] public uint RatingSystemId { get; set; }

    [JsonPropertyName("rating_system_name")]
    public string RatingSystemName { get; set; } = string.Empty;
}
