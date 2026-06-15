using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Identity.Entities;
using TheGameVoice.Web.ViewModels.News;

namespace TheGameVoice.Web.Controllers;

public class NewsController : Controller
{
    private readonly IArticleRepository
        _articleRepository;
    private readonly ICacheService
_cacheService;
    private readonly UserManager<ApplicationUser>
    _userManager;
    public NewsController(
        IArticleRepository articleRepository, ICacheService cacheService, UserManager<ApplicationUser> userManager)
    {
        _articleRepository =
            articleRepository;
        _cacheService = cacheService;
        _userManager = userManager;
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
                    var author =
                         await _userManager
                             .Users
                             .Include(x => x.AvatarImage)
                             .FirstOrDefaultAsync(x =>
                                 x.Id == article.AuthorId);

                    return new ArticleDetailsViewModel
                    {
                        Article = article,

                        RelatedArticles = relatedArticles,

                        AuthorName =
                        author?.FullName
                        ?? "TheGameVoice Editorial Team",

                        AuthorAvatarUrl =
                        author?.AvatarImage?.FilePath


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