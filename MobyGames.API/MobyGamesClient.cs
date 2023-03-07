using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http.Extensions;
using MobyGames.API.DataObjects;
using MobyGames.API.Exceptions;

namespace MobyGames.API;

public sealed class MobyGamesClient : IDisposable
{
    private readonly string apiKey;
    private readonly HttpClient httpClient;

    public static readonly Uri MobyGamesApiUrl = new("https://api.mobygames.com/v1/");

    public MobyGamesClient(string apiKey)
    {
        this.apiKey = apiKey;

        // NOTE: Could not use BaseAddress property with MobyGames, which currently accept the API key only as a query parameter.
        httpClient = new HttpClient();
    }

    #region Endpoints

    public Task<IEnumerable<Genre>> Genres() =>
        GetList<Genre>("genres");

    public Task<IEnumerable<Group>> Groups(MobyGamesClientOptions.CommonOptions? options = null) =>
        GetList<Group>("groups", options);

    public Task<IEnumerable<Platform>> Platforms() => GetList<Platform>("platforms");

    public Task<IEnumerable<Game>> Games(MobyGamesClientOptions.GamesOptions? options = null) =>
        GetList<Game>("games", options);

    public Task<IEnumerable<uint>> RecentGames(MobyGamesClientOptions.RecentGamesOptions? options = null) =>
        GetList<uint>("games/recent", "games", options);

    public Task<IEnumerable<uint>> RandomGames(MobyGamesClientOptions.RandomGamesOptions? options = null) =>
        GetList<uint>("games/random", "games", options);

    public Task<Game> Game(uint gameId, MobyGamesClientOptions.GameOptions? options = null) =>
        Get<Game>($"games/{gameId}", options);

    public Task<IEnumerable<GamePlatform>> GamePlatforms(uint gameId) =>
        GetList<GamePlatform>($"games/{gameId}/platforms", "platforms");

    public Task<GamePlatform> GamePlatform(uint gameId, uint platformId) =>
        Get<GamePlatform>($"games/{gameId}/platforms/{platformId}");

    public Task<IEnumerable<Screenshot>> GameScreenshots(uint gameId, uint platformId) =>
        GetList<Screenshot>($"games/{gameId}/platforms/{platformId}/screenshots", "screenshots");

    public Task<IEnumerable<CoverGroup>> GameCovers(uint gameId, uint platformId) =>
        GetList<CoverGroup>($"games/{gameId}/platforms/{platformId}/covers", "cover_groups");

    #endregion

    private async Task<T> Get<T>(string path, IMobyClientOptions? options = null) =>
        await PerformRequest<T>(path, options);

    private Task<IEnumerable<T>> GetList<T>(string path) => GetList<T>(path, null);

    private Task<IEnumerable<T>> GetList<T>(string path, IMobyClientOptions? options) =>
        GetList<T>(path, path, options);

    private async Task<IEnumerable<T>> GetList<T>(string path, string entityName, IMobyClientOptions? options = null)
    {
        var result = await PerformRequest<JsonObject>(path, options);

        if (result[entityName] is { } node)
        {
            return node.Deserialize<IEnumerable<T>>() ?? Enumerable.Empty<T>();
        }

        return Enumerable.Empty<T>();
    }

    private Uri BuildUrl(string path, IMobyClientOptions? options)
    {
        var queryBuilder = new QueryBuilder { { "api_key", apiKey } };

        options?.Serialize(queryBuilder);

        var builder = new UriBuilder(new Uri(MobyGamesApiUrl, path))
        {
            Query = queryBuilder.ToString()
        };

        return builder.Uri;
    }

    private async Task<T> PerformRequest<T>(string path, IMobyClientOptions? options = null)
    {
        string? content;

        using var response = await httpClient.GetAsync(BuildUrl(path, options));

        if (!response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<MobyGamesErrorResponse>();

            if (result is not null)
            {
                throw new MobyGamesApiException(result);
            }

            content = await response.Content.ReadAsStringAsync();
            throw new MobyGamesException($"An unknown API error has occurred:\n{content}");
        }

        var obj = await response.Content.ReadFromJsonAsync<T>();

        if (obj is null)
        {
            content = await response.Content.ReadAsStringAsync();
            throw new MobyGamesException($"Could not read type {typeof(T).Name} from JSON:\n{content}");
        }

        return obj;
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }
}
