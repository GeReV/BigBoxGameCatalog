using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class CoverGroup
{
    [JsonPropertyName("comments")] public string? Comments { get; set; }

    [JsonPropertyName("countries")] public List<string> Countries { get; set; } = new();

    [JsonPropertyName("covers")] public List<Cover> Covers { get; set; } = new();
}
