using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;

namespace TheGameVoice.Web.Controllers;

public class FeedController : Controller
{
    private readonly IArticleRepository
        _articleRepository;

    public FeedController(
        IArticleRepository articleRepository)
    {
        _articleRepository =
            articleRepository;
    }

    [Route("feed")]
    public async Task<IActionResult> Index()
    {
        var articles =
            await _articleRepository
                .GetLatestPublishedAsync(20);

        var feed =
            new XElement("rss",
                new XAttribute("version", "2.0"),

                new XElement("channel",

                    new XElement("title",
                        "TheGameVoice"),

                    new XElement("link",
                        $"{Request.Scheme}://{Request.Host}"),

                    new XElement("description",
                        "Latest gaming news, reviews and features"),

                    articles.Select(article =>
                        new XElement("item",

                            new XElement("title",
                                article.Title),

                            new XElement("link",
                                $"{Request.Scheme}://{Request.Host}/news/{article.Slug}"),

                            new XElement("description",
                                article.Summary),
                            new XElement(
                                "category",
                                article.Category?.Name),
                            new XElement("pubDate",
                                article.PublishedAt?
                                    .ToUniversalTime()
                                    .ToString("R"))
                        ))
                ));

        var xml =
            new XDocument(
                new XDeclaration(
                    "1.0",
                    "utf-8",
                    "yes"),
                feed);

        return Content(
            xml.ToString(),
            "application/rss+xml",
            Encoding.UTF8);
    }
}