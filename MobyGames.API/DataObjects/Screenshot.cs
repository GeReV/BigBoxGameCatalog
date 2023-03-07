using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class Screenshot
{
    [JsonPropertyName("caption")] public string Caption { get; set; }

    [JsonPropertyName("image")] public Uri Image { get; set; }

    [JsonPropertyName("thumbnail_image")] public Uri ThumbnailImage { get; set; }

    [JsonPropertyName("width")] public uint Width { get; set; }

    [JsonPropertyName("height")] public uint Height { get; set; }
}
