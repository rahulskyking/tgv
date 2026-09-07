using TheGameVoice.Domain.Common.Extensions;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Web.Services;

/// <summary>
/// Resolves which site mode (PC / Console gaming or Mobile gaming) the
/// current visitor is browsing.
///
/// Resolution order:
///   1. an explicit <c>?mode=</c> query value (used by the header toggle),
///   2. the <c>tgv_mode</c> cookie set the last time the reader flipped,
///   3. PC / Console — the historical behaviour of the site.
/// </summary>
public interface ISiteSegmentAccessor
{
    /// <summary>Site mode for the current request.</summary>
    GameSegment Current { get; }

    /// <summary>True when the reader is in mobile gaming mode.</summary>
    bool IsMobile { get; }

    /// <summary>Persists the chosen mode in the visitor's cookie.</summary>
    void Set(GameSegment segment);
}

public class SiteSegmentAccessor : ISiteSegmentAccessor
{
    public const string CookieName = "tgv_mode";

    public const string QueryKey = "mode";

    private readonly IHttpContextAccessor _httpContextAccessor;

    private GameSegment? _resolved;

    public SiteSegmentAccessor(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public GameSegment Current
    {
        get
        {
            if (_resolved.HasValue)
            {
                return _resolved.Value;
            }

            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                _resolved = GameSegment.PcConsole;

                return _resolved.Value;
            }

            // 1. explicit override on the URL
            if (context.Request.Query.TryGetValue(
                    QueryKey,
                    out var fromQuery) &&
                !string.IsNullOrWhiteSpace(fromQuery))
            {
                _resolved = GameSegmentExtensions
                    .ParseSegment(fromQuery.ToString());

                return _resolved.Value;
            }

            // 2. cookie
            if (context.Request.Cookies.TryGetValue(
                    CookieName,
                    out var fromCookie) &&
                !string.IsNullOrWhiteSpace(fromCookie))
            {
                _resolved = GameSegmentExtensions
                    .ParseSegment(fromCookie);

                return _resolved.Value;
            }

            // 3. default
            _resolved = GameSegment.PcConsole;

            return _resolved.Value;
        }
    }

    public bool IsMobile
        => Current == GameSegment.Mobile;

    public void Set(GameSegment segment)
    {
        if (segment != GameSegment.Mobile)
        {
            segment = GameSegment.PcConsole;
        }

        _resolved = segment;

        var context = _httpContextAccessor.HttpContext;

        if (context == null)
        {
            return;
        }

        context.Response.Cookies.Append(
            CookieName,
            segment.ToSlug(),
            new CookieOptions
            {
                // Remembering the reader's choice is part of the requested
                // feature, so the cookie is functionally essential.
                IsEssential = true,
                HttpOnly = false,
                SameSite = SameSiteMode.Lax,
                Secure = context.Request.IsHttps,
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                Path = "/"
            });
    }
}
