using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http.Extensions;
using MobyGames.API.DataObjects;
using MobyGames.API.Exceptions;

namespace MobyGames.API;

public sealed class MobyGamesClient : IDisposable
{
    private readonly string apiKey;

    private readonly HttpClient httpClient;

    private readonly SlidingWindowRateLimiter rateLimiter = new(
        new SlidingWindowRateLimiterOptions
        {
            Window = TimeSpan.FromMilliseconds(2000),
            SegmentsPerWindow = 1,
            PermitLimit = 1,
            QueueLimit = 10,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        }
    );

    public static readonly Uri MobyGamesApiUrl = new("https://api.mobygames.com/v1/");

    public MobyGamesClient(string apiKey)
    {
        this.apiKey = apiKey;

        // NOTE: Could not use BaseAddress property with MobyGames, which currently accept the API key only as a query parameter.
        httpClient = new HttpClient();
    }

    #region Endpoints

    public Task<IEnumerable<Genre>> Genres(CancellationToken cancellationToken = default) =>
        GetList<Genre>("genres", cancellationToken: cancellationToken);

    public Task<IEnumerable<Group>> Groups(
        MobyGamesClientOptions.CommonOptions? options = null,
        CancellationToken cancellationToken = default
    ) =>
        GetList<Group>("groups", options, cancellationToken);

    public Task<IEnumerable<Platform>> Platforms(CancellationToken cancellationToken = default) =>
        GetList<Platform>("platforms", cancellationToken: cancellationToken);

    public Task<IEnumerable<Game>> Games(
        MobyGamesClientOptions.GamesOptions? options = null,
        CancellationToken cancellationToken = default
    ) =>
        GetList<Game>("games", options, cancellationToken);

    public Task<IEnumerable<uint>> RecentGames(
        MobyGamesClientOptions.RecentGamesOptions? options = null,
        CancellationToken cancellationToken = default
    ) =>
        GetList<uint>("games/recent", "games", options, cancellationToken);

    public Task<IEnumerable<uint>> RandomGames(
        MobyGamesClientOptions.RandomGamesOptions? options = null,
        CancellationToken cancellationToken = default
    ) =>
        GetList<uint>("games/random", "games", options, cancellationToken);

    public Task<Game> Game(
        uint gameId,
        MobyGamesClientOptions.GameOptions? options = null,
        CancellationToken cancellationToken = default
    ) =>
        Get<Game>($"games/{gameId}", options, cancellationToken);

    public Task<IEnumerable<GamePlatform>> GamePlatforms(uint gameId, CancellationToken cancellationToken = default) =>
        GetList<GamePlatform>($"games/{gameId}/platforms", "platforms", cancellationToken: cancellationToken);

    public Task<GamePlatform> GamePlatform(
        uint gameId,
        uint platformId,
        CancellationToken cancellationToken = default
    ) =>
        Get<GamePlatform>($"games/{gameId}/platforms/{platformId}", cancellationToken: cancellationToken);

    public Task<IEnumerable<Screenshot>> GameScreenshots(
        uint gameId,
        uint platformId,
        CancellationToken cancellationToken = default
    ) =>
        GetList<Screenshot>(
            $"games/{gameId}/platforms/{platformId}/screenshots",
            "screenshots",
            cancellationToken: cancellationToken
        );

    public Task<IEnumerable<CoverGroup>> GameCovers(uint gameId, uint platformId) =>
        GetList<CoverGroup>($"games/{gameId}/platforms/{platformId}/covers", "cover_groups");

    #endregion

    private async Task<T> Get<T>(
        string path,
        IMobyClientOptions? options = null,
        CancellationToken cancellationToken = default
    ) =>
        await PerformRequest<T>(path, options, cancellationToken);

    private Task<IEnumerable<T>> GetList<T>(
        string path,
        IMobyClientOptions? options = null,
        CancellationToken cancellationToken = default
    ) =>
        GetList<T>(path, path, options, cancellationToken);

    private async Task<IEnumerable<T>> GetList<T>(
        string path,
        string entityName,
        IMobyClientOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        var result = await PerformRequest<JsonNode>(path, options, cancellationToken);

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

    private async Task<T> PerformRequest<T>(
        string path,
        IMobyClientOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new MobyGamesException("Missing MobyGames API key");
        }

        string? content;

        using var lease = await rateLimiter.AcquireAsync(cancellationToken: cancellationToken);
        if (!lease.IsAcquired)
        {
            throw new MobyGamesException("Failed to acquire rate limit lease");
        }

        var requestUri = BuildUrl(path, options);

        using var response = await httpClient.GetAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var result =
                await response.Content.ReadFromJsonAsync<MobyGamesErrorResponse>(cancellationToken: cancellationToken);

            if (result is not null)
            {
                throw new MobyGamesApiException(result);
            }

            content = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new MobyGamesException($"An unknown API error has occurred:\n{content}");
        }

        Debug.WriteLine(
            $"MobyGames request {requestUri}:\n{JsonSerializer.Deserialize<JsonNode>(await response.Content.ReadAsStringAsync(cancellationToken))?.ToJsonString(new JsonSerializerOptions { WriteIndented = true })}\n\n"
        );

        var obj = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);

        if (obj is null)
        {
            content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new MobyGamesException($"Could not read type {typeof(T).Name} from JSON:\n{content}");
        }

        return obj;
    }

    public void Dispose()
    {
        httpClient.Dispose();
        rateLimiter.Dispose();
    }
}
