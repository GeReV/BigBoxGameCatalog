using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class Cover
{
    [JsonPropertyName("comments")] public string? Comments { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("image")] public Uri Image { get; set; }

    [JsonPropertyName("thumbnail_image")] public Uri ThumbnailImage { get; set; }

    [JsonPropertyName("width")] public uint Width { get; set; }

    [JsonPropertyName("height")] public uint Height { get; set; }

    [JsonPropertyName("scan_of")] public string? ScanOf { get; set; }
}
