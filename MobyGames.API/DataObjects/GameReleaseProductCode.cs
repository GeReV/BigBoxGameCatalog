using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class GameReleaseProductCode
{
    [JsonPropertyName("product_code_type_id")]
    public uint ProductCodeTypeId { get; set; }

    [JsonPropertyName("product_code_type")]
    public string ProductCodeType { get; set; } = string.Empty;

    [JsonPropertyName("product_code")] public string ProductCode { get; set; } = string.Empty;
}
