using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record Screenshot
{
    [JsonPropertyName("caption")] public string Caption { get; init; } = string.Empty;

    [JsonPropertyName("image")] public Uri Image { get; init; }

    [JsonPropertyName("thumbnail_image")] public Uri ThumbnailImage { get; init; }

    [JsonPropertyName("width")] public uint Width { get; init; }

    [JsonPropertyName("height")] public uint Height { get; init; }
}
