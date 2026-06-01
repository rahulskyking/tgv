using Microsoft.AspNetCore.Mvc;
using System.Text;
using TheGameVoice.Application.Interfaces.Persistence;

namespace TheGameVoice.Web.Controllers;

public class SitemapController : Controller
{
    private readonly IArticleRepository _articleRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IGameRepository _gameRepository;

    public SitemapController(
        IArticleRepository articleRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        IGameRepository gameRepository)
    {
        _articleRepository = articleRepository;
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
        _gameRepository = gameRepository;
    }

    [Route("sitemap.xml")]
    public async Task<IActionResult> Index()
    {
        var sb = new StringBuilder();

        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

        // URLs will be added next
        sb.AppendLine($"""
<url>
    <loc>{Request.Scheme}://{Request.Host}</loc>
</url>
""");
        var articles =
    await _articleRepository
        .GetPublishedAsync();

        foreach (var article in articles)
        {
            sb.AppendLine($"""
<url>
    <loc>
        {Request.Scheme}://{Request.Host}/news/{article.Slug}
    </loc>
</url>
""");
        }
        var categories =
    await _categoryRepository
        .GetAllAsync();

        foreach (var category in categories)
        {
            sb.AppendLine($"""
<url>
    <loc>
        {Request.Scheme}://{Request.Host}/category/{category.Slug}
    </loc>
</url>
""");
        }
        var tags =
    await _tagRepository
        .GetAllAsync();

        foreach (var tag in tags)
        {
            sb.AppendLine($"""
<url>
    <loc>
        {Request.Scheme}://{Request.Host}/tag/{tag.Slug}
    </loc>
</url>
""");
        }
        var games =
    await _gameRepository
        .GetAllAsync();

        foreach (var game in games)
        {
            sb.AppendLine($"""
<url>
    <loc>
        {Request.Scheme}://{Request.Host}/game/{game.Slug}
    </loc>
</url>
""");
        }
        sb.AppendLine("</urlset>");

        return Content(
            sb.ToString(),
            "application/xml");
    }
}