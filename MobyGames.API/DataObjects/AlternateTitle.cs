using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class AlternateTitle
{
    [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
}
