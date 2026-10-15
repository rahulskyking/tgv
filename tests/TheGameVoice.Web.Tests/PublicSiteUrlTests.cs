using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheGameVoice.Web.DependencyInjection;
using TheGameVoice.Web.Services;
using Xunit;

namespace TheGameVoice.Web.Tests;

public class PublicSiteUrlTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("http://thegamevoice.com")]
    [InlineData("/relative")]
    [InlineData("https://thegamevoice.com/subpath")]
    [InlineData("https://thegamevoice.com/?mode=mobile")]
    [InlineData("https://thegamevoice.com/#fragment")]
    [InlineData("https://user:password@thegamevoice.com")]
    public void RejectsMissingOrInvalidPublicOrigin(string? value)
    {
        Assert.Throws<InvalidOperationException>(() => new PublicSiteUrl(Configuration(value)));
    }

    [Theory]
    [InlineData("https://thegamevoice.com", "thegamevoice.com")]
    [InlineData("https://www.thegamevoice.com/", "www.thegamevoice.com")]
    [InlineData("https://thegamevoice.com:8443", "thegamevoice.com:8443")]
    public void SuppliesConfiguredHttpsOriginToMvc(string origin, string host)
    {
        var service = new PublicSiteUrl(Configuration(origin));
        var url = new CapturingUrlHelper();
        var values = new { slug = "game-update" };

        var result = service.Action(url, "Details", "News", values);

        Assert.Equal("generated-by-mvc", result);
        Assert.Equal("https", url.LastAction!.Protocol);
        Assert.Equal(host, url.LastAction.Host);
        Assert.Equal("Details", url.LastAction.Action);
        Assert.Equal("News", url.LastAction.Controller);
        Assert.Same(values, url.LastAction.Values);
    }

    [Fact]
    public void ThrowsWhenMvcCannotGenerateARoute()
    {
        var service = new PublicSiteUrl(Configuration("https://thegamevoice.com"));
        Assert.Throws<InvalidOperationException>(() => service.Action(
            new CapturingUrlHelper { GeneratedUrl = null }, "Details", "News"));
    }

    [Fact]
    public void WebRegistrationResolvesPublicUrlServiceAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(Configuration("https://thegamevoice.com"));
        services.AddWebServices();
        using var provider = services.BuildServiceProvider();

        Assert.Same(provider.GetRequiredService<PublicSiteUrl>(),
            provider.GetRequiredService<PublicSiteUrl>());
        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(ISiteSegmentAccessor)
            && descriptor.Lifetime == ServiceLifetime.Scoped);
    }

    private static IConfiguration Configuration(string? origin) =>
        new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?> { ["Site:BaseUrl"] = origin }).Build();

    private sealed class CapturingUrlHelper : IUrlHelper
    {
        public ActionContext ActionContext { get; } = new();
        public UrlActionContext? LastAction { get; private set; }
        public string? GeneratedUrl { get; init; } = "generated-by-mvc";

        public string? Action(UrlActionContext actionContext)
        {
            LastAction = actionContext;
            return GeneratedUrl;
        }

        public string? Content(string? contentPath) => throw new NotSupportedException();
        public bool IsLocalUrl(string? url) => throw new NotSupportedException();
        public string? Link(string? routeName, object? values) => throw new NotSupportedException();
        public string? RouteUrl(UrlRouteContext routeContext) => throw new NotSupportedException();
    }
}
