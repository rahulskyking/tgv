using System.Reflection;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Web.Controllers;
using Xunit;

namespace TheGameVoice.Web.Tests;

public class DiscoveryLinkTests
{
    [Fact]
    public async Task SitemapUsesArticleRouteAndEscapesXml()
    {
        var articleRepository = Stub<IArticleRepository>("GetPublishedAsync",
            Task.FromResult<IReadOnlyList<Article>>(new[]
            {
                new Article { Slug = "game-update" }
            }));
        var controller = new SitemapController(
            articleRepository,
            Stub<ICategoryRepository>("GetAllAsync", Task.FromResult<IReadOnlyList<Category>>(
                new[] { new Category { Slug = "news" } })),
            Stub<ITagRepository>("GetAllAsync", Task.FromResult<IReadOnlyList<Tag>>(
                new[] { new Tag { Slug = "pc" } })),
            Stub<IGameRepository>("GetAllAsync", Task.FromResult<IReadOnlyList<Game>>(
                new[] { new Game { Slug = "example-game" } })));
        Configure(controller);
        // An ampersand in a generated URL must not break sitemap XML.
        controller.Url = new TestUrlHelper { QuerySuffix = "?a=1&b=2" };

        var result = Assert.IsType<ContentResult>(await controller.Index());
        Assert.StartsWith("application/xml", result.ContentType!);
        var document = XDocument.Parse(result.Content!);
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        Assert.Equal(ns + "urlset", document.Root!.Name);
        var urls = document.Descendants(ns + "loc").Select(node => node.Value).ToArray();

        Assert.Equal(5, urls.Length);
        Assert.Contains("https://thegamevoice.com/article/game-update?a=1&b=2", urls);
        Assert.Contains("https://thegamevoice.com/category/news?a=1&b=2", urls);
        Assert.Contains("https://thegamevoice.com/tag/pc?a=1&b=2", urls);
        Assert.Contains("https://thegamevoice.com/game/example-game?a=1&b=2", urls);
        Assert.DoesNotContain("/news/", result.Content!);
        Assert.Contains("&amp;", result.Content!);
    }

    [Fact]
    public async Task FeedUsesArticleRouteAndPreservesEscapedContent()
    {
        var controller = new FeedController(Stub<IArticleRepository>(
            "GetLatestPublishedAsync",
            Task.FromResult<IReadOnlyList<Article>>(new[]
            {
                new Article
                {
                    Slug = "game-update",
                    Title = "News & reviews",
                    Summary = "<p>Game update</p>",
                    PublishedAt = new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc)
                }
            })));
        Configure(controller);

        var result = Assert.IsType<ContentResult>(await controller.Index());
        var document = XDocument.Parse(result.Content!);
        var item = Assert.Single(document.Descendants("item"));
        Assert.Equal("https://thegamevoice.com/article/game-update", item.Element("link")!.Value);
        Assert.Equal("News & reviews", item.Element("title")!.Value);
        Assert.Equal("<p>Game update</p>", item.Element("description")!.Value);
        Assert.DoesNotContain("/news/", result.Content!);
    }

    [Fact]
    public void LegacyArticleLinkPermanentlyRedirectsToDetails()
    {
        // This action only delegates to routing and needs no repository services.
        var controller = new NewsController(null!, null!, null!, null!);
        var result = Assert.IsType<RedirectToActionResult>(controller.LegacyDetails("game-update"));

        Assert.True(result.Permanent);
        Assert.Equal("Details", result.ActionName);
        Assert.Equal("News", result.ControllerName);
        Assert.Equal("game-update", result.RouteValues!["slug"]);

        var route = typeof(NewsController).GetMethod(nameof(NewsController.LegacyDetails))!
            .GetCustomAttribute<HttpGetAttribute>()!;
        Assert.Equal("news/{slug}", route.Template);
        Assert.Equal(100, route.Order);
    }

    private static void Configure(Controller controller)
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("thegamevoice.com");
        controller.ControllerContext = new ControllerContext { HttpContext = context };
        controller.Url = new TestUrlHelper();
    }

    private static T Stub<T>(string method, object result) where T : class
    {
        var proxy = DispatchProxy.Create<T, RepositoryStub>();
        var stub = (RepositoryStub)(object)proxy;
        stub.Method = method;
        stub.Result = result;
        return proxy;
    }

    public class RepositoryStub : DispatchProxy
    {
        public string Method { get; set; } = string.Empty;
        public object? Result { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            Assert.Equal(Method, targetMethod!.Name);
            // Sitemap/feed discovery must not depend on a visitor's segment cookie.
            if (targetMethod.Name == "GetPublishedAsync")
            {
                Assert.Null(Assert.Single(args!));
            }
            else if (targetMethod.Name == "GetLatestPublishedAsync")
            {
                Assert.Equal(20, args![0]);
                Assert.Null(args[1]);
            }
            return Result;
        }
    }

    // Verifies the controller asks MVC for the correct action, rather than
    // concatenating an obsolete path. Actual endpoint routing still needs a live check.
    private sealed class TestUrlHelper : IUrlHelper
    {
        public ActionContext ActionContext { get; } = new();
        public string QuerySuffix { get; init; } = string.Empty;

        public string? Action(UrlActionContext context)
        {
            Assert.Equal("https", context.Protocol);
            var prefix = context.Controller switch
            {
                "Home" => "",
                "News" => "article",
                "Categories" => "category",
                "Tags" => "tag",
                "Games" => "game",
                _ => throw new InvalidOperationException("Unexpected controller.")
            };
            Assert.Equal(prefix.Length == 0 ? "Index" : "Details", context.Action);
            var values = new RouteValueDictionary(context.Values);
            var path = prefix.Length == 0 ? "/" : $"/{prefix}/{values["slug"]}";
            return "https://thegamevoice.com" + path + QuerySuffix;
        }

        public string? Content(string? contentPath) => throw new NotSupportedException();
        public bool IsLocalUrl(string? url) => throw new NotSupportedException();
        public string? Link(string? routeName, object? values) => throw new NotSupportedException();
        public string? RouteUrl(UrlRouteContext routeContext) => throw new NotSupportedException();
    }
}
