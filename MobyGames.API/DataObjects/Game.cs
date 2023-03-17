using System.Text.Json.Serialization;

namespace MobyGames.API.DataObjects;

public sealed record Game
{
    [JsonPropertyName("game_id")] public uint Id { get; init; }

    [JsonPropertyName("title")] public string Title { get; init; } = string.Empty;

    [JsonPropertyName("moby_url")] public Uri? MobyUrl { get; init; }

    [JsonPropertyName("moby_score")] public double? MobyScore { get; init; }

    [JsonPropertyName("num_votes")] public uint? NumVotes { get; init; }

    [JsonPropertyName("official_url")] public Uri? OfficialUrl { get; init; }

    [JsonPropertyName("alternate_titles")] public List<AlternateTitle> AlternateTitles { get; init; } = new();

    [JsonPropertyName("genres")] public List<Genre> Genres { get; init; } = new();

    [JsonPropertyName("platforms")] public List<GamePlatform> Platforms { get; init; } = new();

    [JsonPropertyName("sample_cover")] public Cover? SampleCover { get; init; }

    [JsonPropertyName("sample_screenshots")]
    public List<Screenshot> SampleScreenshots { get; init; } = new();
}
