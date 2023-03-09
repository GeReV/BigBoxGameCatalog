using System.Text.Json;
using System.Text.Json.Serialization;
using MobyGames.API.DataObjects;
using MobyGames.API.Helpers;

namespace MobyGames.API;

public class JsonCoverScanConverter : JsonConverter<CoverScanOf?>
{
    public override CoverScanOf? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (str is null)
        {
            return null;
        }

        return Enum.GetValues<CoverScanOf>()
            .FirstOrDefault(v => v.GetDescription().Equals(str, StringComparison.InvariantCultureIgnoreCase));
    }

    public override void Write(Utf8JsonWriter writer, CoverScanOf? value, JsonSerializerOptions options)
    {
        writer.WriteString("scan_of", value?.GetDescription());
    }
}
