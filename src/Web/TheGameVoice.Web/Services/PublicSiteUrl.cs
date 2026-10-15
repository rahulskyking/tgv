using Microsoft.AspNetCore.Mvc;

namespace TheGameVoice.Web.Services;

/// <summary>
/// Generates public SEO URLs using the configured origin, not the proxy's
/// internal request scheme or an incoming Host header.
/// </summary>
public sealed class PublicSiteUrl
{
    private readonly Uri _origin;

    public PublicSiteUrl(IConfiguration configuration)
    {
        var value = configuration["Site:BaseUrl"];

        if (!Uri.TryCreate(value, UriKind.Absolute, out var origin)
            || origin.Scheme != Uri.UriSchemeHttps
            || string.IsNullOrWhiteSpace(origin.Host)
            || origin.AbsolutePath != "/"
            || !string.IsNullOrEmpty(origin.Query)
            || !string.IsNullOrEmpty(origin.Fragment)
            || !string.IsNullOrEmpty(origin.UserInfo))
        {
            throw new InvalidOperationException(
                "Site:BaseUrl must be an HTTPS origin, " +
                "for example https://thegamevoice.com");
        }

        _origin = origin;
    }

    public string Action(
        IUrlHelper url,
        string action,
        string controller,
        object? values = null)
    {
        return url.Action(
            action,
            controller,
            values,
            protocol: _origin.Scheme,
            host: _origin.Authority)
            ?? throw new InvalidOperationException(
                "Unable to generate the public URL.");
    }
}
