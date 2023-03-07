using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class AlternateTitle
{
    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }
}
