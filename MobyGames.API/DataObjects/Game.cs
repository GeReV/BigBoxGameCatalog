using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public class Game
{
    [JsonPropertyName("game_id")] public uint Id { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("moby_url")] public Uri MobyUrl { get; set; }

    [JsonPropertyName("moby_score")] public double MobyScore { get; set; }

    [JsonPropertyName("num_votes")] public uint NumVotes { get; set; }

    [JsonPropertyName("official_url")] public Uri? OfficialUrl { get; set; }

    [JsonPropertyName("alternate_titles")] public List<AlternateTitle> AlternateTitles { get; set; } = new();

    [JsonPropertyName("genres")] public List<Genre> Genres { get; set; } = new();

    [JsonPropertyName("platforms")] public List<GamePlatform> Platforms { get; set; } = new();

    [JsonPropertyName("sample_cover")] public Cover? SampleCover { get; set; }

    [JsonPropertyName("sample_screenshots")]
    public List<Screenshot> SampleScreenshots { get; set; } = new();
}
