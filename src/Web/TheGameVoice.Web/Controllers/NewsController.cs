using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Web.ViewModels.News;

namespace TheGameVoice.Web.Controllers;

public class NewsController : Controller
{
    private readonly IArticleRepository
        _articleRepository;
    private readonly ICacheService
_cacheService;
    public NewsController(
        IArticleRepository articleRepository, ICacheService cacheService)
    {
        _articleRepository =
            articleRepository;
        _cacheService = cacheService;
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
        var model =
            await _cacheService.GetOrCreateAsync(
                $"article_{slug}",
                async () =>
                {
                    var article =
                        await _articleRepository
                            .GetBySlugAsync(slug);

                    if (article == null
                        || article.Status != ArticleStatus.Published)
                    {
                        return null!;
                    }

                    var relatedArticles =
                        await _articleRepository
                            .GetRelatedArticlesAsync(
                                article.CategoryId,
                                article.Id);

                    return new ArticleDetailsViewModel
                    {
                        Article = article,
                        RelatedArticles = relatedArticles
                    };
                },
                CacheDurations.Short);

        if (model == null)
        {
            return NotFound();
        }

        ViewData["CanonicalUrl"] =
            Url.Action(
                "Details",
                "News",
                new { slug = model.Article.Slug },
                Request.Scheme);

        return View(model);
    }
}