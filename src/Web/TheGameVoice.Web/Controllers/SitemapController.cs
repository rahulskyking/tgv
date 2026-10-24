using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Linq;
using TheGameVoice.Application.Interfaces.Persistence;

namespace TheGameVoice.Web.Controllers;

public class SitemapController : Controller
{
    private readonly IArticleRepository _articleRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IEventRepository _eventRepository;

    public SitemapController(
        IArticleRepository articleRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        IGameRepository gameRepository,
        IEventRepository eventRepository)
    {
        _articleRepository = articleRepository;
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
        _gameRepository = gameRepository;
        _eventRepository = eventRepository;
    }

    [Route("sitemap.xml")]
    public async Task<IActionResult> Index()
    {
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var urlset = new XElement(ns + "urlset");

        void AddUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidOperationException("Unable to generate a sitemap URL.");
            }

            // XElement escapes URL characters such as '&' correctly.
            urlset.Add(new XElement(ns + "url", new XElement(ns + "loc", url)));
        }

        AddUrl(Url.Action("Index", "Home", null, Request.Scheme));

        // No segment filter: discover published PC/console AND mobile articles.
        var articles = await _articleRepository.GetPublishedAsync();
        foreach (var article in articles)
        {
            AddUrl(Url.Action("Details", "News", new { slug = article.Slug }, Request.Scheme));
        }

        var categories = await _categoryRepository.GetAllAsync();
        foreach (var category in categories)
        {
            AddUrl(Url.Action("Details", "Categories", new { slug = category.Slug }, Request.Scheme));
        }

        var tags = await _tagRepository.GetAllAsync();
        foreach (var tag in tags)
        {
            AddUrl(Url.Action("Details", "Tags", new { slug = tag.Slug }, Request.Scheme));
        }

        var games = await _gameRepository.GetAllAsync();
        foreach (var game in games)
        {
            AddUrl(Url.Action("Details", "Games", new { slug = game.Slug }, Request.Scheme));
        }

        AddUrl(Url.Action("Index", "Events", null, Request.Scheme));
        foreach (var gamingEvent in await _eventRepository.GetAllAsync())
        {
            AddUrl(Url.Action("Details", "Events", new { slug = gamingEvent.Slug }, Request.Scheme));
        }

        var xml = new XDocument(new XDeclaration("1.0", "utf-8", null), urlset);
        return Content(xml.ToString(), "application/xml", Encoding.UTF8);
    }
}
