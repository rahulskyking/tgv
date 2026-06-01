using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Web.ViewModels.News;

namespace TheGameVoice.Web.Controllers;

public class NewsController : Controller
{
    private readonly IArticleRepository
        _articleRepository;

    public NewsController(
        IArticleRepository articleRepository)
    {
        _articleRepository =
            articleRepository;
    }

    public async Task<IActionResult> Index()
    {
        var articles =
            await _articleRepository
                .GetPublishedAsync();

        return View(articles);
    }

    [Route("news/{slug}")]
    public async Task<IActionResult> Details(
       string slug)
    {
        var article =
            await _articleRepository
                .GetBySlugAsync(slug);

        if (article == null
            || article.Status != ArticleStatus.Published)
        {
            return NotFound();
        }

        var relatedArticles =
            await _articleRepository
                .GetRelatedArticlesAsync(
                    article.CategoryId,
                    article.Id);

        var model =
            new ArticleDetailsViewModel
            {
                Article = article,
                RelatedArticles = relatedArticles
            };

        ViewData["CanonicalUrl"] =
            Url.Action(
                "Details",
                "News",
                new { slug = article.Slug },
                Request.Scheme);

        return View(model);
    }
}