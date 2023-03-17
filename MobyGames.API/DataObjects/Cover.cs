using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record Cover
{
    [JsonPropertyName("comments")] public string? Comments { get; init; }

    [JsonPropertyName("description")] public string? Description { get; init; }

    [JsonPropertyName("image")] public Uri Image { get; init; }

    [JsonPropertyName("thumbnail_image")] public Uri ThumbnailImage { get; init; }

    [JsonPropertyName("width")] public uint Width { get; init; }

    [JsonPropertyName("height")] public uint Height { get; init; }

    [JsonPropertyName("scan_of")]
    [JsonConverter(typeof(JsonCoverScanConverter))]
    public CoverScanOf? ScanOf { get; init; }
}
