using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record GameReleaseProductCode
{
    [JsonPropertyName("product_code_type_id")]
    public uint ProductCodeTypeId { get; init; }

    [JsonPropertyName("product_code_type")]
    public string ProductCodeType { get; init; } = string.Empty;

    [JsonPropertyName("product_code")] public string ProductCode { get; init; } = string.Empty;
}
