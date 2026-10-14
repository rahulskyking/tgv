using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheGameVoice.Application.Common.Games;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Infrastructure.Configuration;

namespace TheGameVoice.Infrastructure.Services;

public sealed class IgdbGameMetadataProvider : IGameMetadataProvider
{
    private const string TokenCacheKey = "igdb-app-token";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly IgdbOptions _options;
    private readonly ILogger<IgdbGameMetadataProvider> _logger;

    public IgdbGameMetadataProvider(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        IOptions<IgdbOptions> options,
        ILogger<IgdbGameMetadataProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_options.ClientId) &&
        !string.IsNullOrWhiteSpace(_options.ClientSecret);

    public async Task<IReadOnlyList<GameMetadata>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException("IGDB credentials are not configured.");
        }

        var token = await GetTokenAsync(cancellationToken);
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(12);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(new Uri(_options.BaseUrl), "games"));

        request.Headers.Add("Client-ID", _options.ClientId);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var safeQuery = query
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal)
            .Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal);

        var apiQuery = $"""
            search "{safeQuery}";
            fields id,name,summary,storyline,first_release_date,
                cover.image_id,artworks.image_id,screenshots.image_id,
                platforms.name,genres.name,
                involved_companies.developer,involved_companies.publisher,
                involved_companies.company.name,websites.url;
            where version_parent = null;
            limit 8;
            """;

        request.Content = new StringContent(apiQuery, Encoding.UTF8, "text/plain");

        using var response = await client.SendAsync(request, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "IGDB game search failed with status {StatusCode}: {Response}",
                (int)response.StatusCode,
                payload.Length > 500 ? payload[..500] : payload);

            throw new HttpRequestException("IGDB could not complete the search.");
        }

        using var document = JsonDocument.Parse(payload);
        return document.RootElement.EnumerateArray()
            .Select(MapGame)
            .ToList();
    }

    private async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(TokenCacheKey, out string? cached) &&
            !string.IsNullOrWhiteSpace(cached))
        {
            return cached;
        }

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _options.ClientId!,
            ["client_secret"] = _options.ClientSecret!,
            ["grant_type"] = "client_credentials"
        });

        using var response = await client.PostAsync(
            _options.TokenUrl,
            content,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(cancellationToken));

        var token = document.RootElement.GetProperty("access_token").GetString()
            ?? throw new InvalidOperationException("IGDB returned an empty access token.");
        var expiresIn = document.RootElement.GetProperty("expires_in").GetInt32();
        var cacheFor = TimeSpan.FromSeconds(Math.Max(60, expiresIn - 300));

        _cache.Set(TokenCacheKey, token, cacheFor);
        return token;
    }

    private static GameMetadata MapGame(JsonElement game)
    {
        var platforms = Names(game, "platforms");
        var genres = Names(game, "genres");
        var websites = Strings(game, "websites", "url");
        var steamUrl = websites.FirstOrDefault(x =>
            x.Contains("store.steampowered.com/app/", StringComparison.OrdinalIgnoreCase));

        return new GameMetadata
        {
            ExternalId = game.GetProperty("id").GetInt64(),
            Name = Text(game, "name") ?? "Unknown game",
            Summary = Text(game, "summary"),
            Description = Text(game, "storyline") ?? Text(game, "summary"),
            ReleaseDate = ReleaseDate(game),
            Developer = CompanyNames(game, "developer"),
            Publisher = CompanyNames(game, "publisher"),
            Platforms = platforms,
            Genres = genres,
            OfficialWebsite = websites.FirstOrDefault(x =>
                !x.Contains("store.steampowered.com", StringComparison.OrdinalIgnoreCase)),
            SteamUrl = steamUrl,
            SteamAppId = ParseSteamAppId(steamUrl),
            CoverUrl = ImageUrl(game, "cover", "cover_big_2x"),
            BannerUrl = ImageUrl(game, "artworks", "screenshot_big") ??
                        ImageUrl(game, "screenshots", "screenshot_big"),
            IsMobile = platforms.Any(IsMobilePlatform),
            IsPcConsole = platforms.Count == 0 || platforms.Any(x => !IsMobilePlatform(x))
        };
    }

    private static string? Text(JsonElement element, string property) =>
        element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static DateOnly? ReleaseDate(JsonElement game)
    {
        if (!game.TryGetProperty("first_release_date", out var value) ||
            !value.TryGetInt64(out var seconds))
        {
            return null;
        }

        return DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime);
    }

    private static List<string> Names(JsonElement game, string property) =>
        game.TryGetProperty(property, out var values) && values.ValueKind == JsonValueKind.Array
            ? values.EnumerateArray()
                .Select(x => Text(x, "name"))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Cast<string>()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
            : new List<string>();

    private static List<string> Strings(JsonElement game, string property, string child) =>
        game.TryGetProperty(property, out var values) && values.ValueKind == JsonValueKind.Array
            ? values.EnumerateArray()
                .Select(x => Text(x, child))
                .Where(x => Uri.TryCreate(x, UriKind.Absolute, out _))
                .Cast<string>()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
            : new List<string>();

    private static string? CompanyNames(JsonElement game, string role)
    {
        if (!game.TryGetProperty("involved_companies", out var companies) ||
            companies.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var names = companies.EnumerateArray()
            .Where(x => x.TryGetProperty(role, out var flag) && flag.GetBoolean())
            .Select(x => x.TryGetProperty("company", out var company)
                ? Text(company, "name")
                : null)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Cast<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase);

        return string.Join(", ", names);
    }

    private static string? ImageUrl(JsonElement game, string property, string size)
    {
        if (!game.TryGetProperty(property, out var value)) return null;
        var item = value.ValueKind == JsonValueKind.Array
            ? value.EnumerateArray().FirstOrDefault()
            : value;

        if (item.ValueKind != JsonValueKind.Object) return null;

        var imageId = Text(item, "image_id");
        return string.IsNullOrWhiteSpace(imageId)
            ? null
            : $"https://images.igdb.com/igdb/image/upload/t_{size}/{imageId}.jpg";
    }

    private static int? ParseSteamAppId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var marker = "/app/";
        var index = url.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (index < 0) return null;
        var value = new string(url[(index + marker.Length)..]
            .TakeWhile(char.IsDigit).ToArray());
        return int.TryParse(value, out var id) ? id : null;
    }

    private static bool IsMobilePlatform(string platform) =>
        platform.Contains("Android", StringComparison.OrdinalIgnoreCase) ||
        platform.Contains("iOS", StringComparison.OrdinalIgnoreCase) ||
        platform.Contains("iPhone", StringComparison.OrdinalIgnoreCase) ||
        platform.Contains("iPad", StringComparison.OrdinalIgnoreCase);
}
