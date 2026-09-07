using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Common.Extensions;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Identity.Entities;
using TheGameVoice.Web.Services;
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

    private readonly ISiteSegmentAccessor _siteSegment;

    public NewsController(
        IArticleRepository articleRepository,
        ICacheService cacheService,
        UserManager<ApplicationUser> userManager,
        ISiteSegmentAccessor siteSegment)
    {
        _articleRepository =
            articleRepository;
        _cacheService = cacheService;
        _userManager = userManager;
        _siteSegment = siteSegment;
    }

    public async Task<IActionResult> Index()
    {
        var articles =
            await _articleRepository
                .GetPublishedAsync(_siteSegment.Current);

        return View(articles);
    }

    // [Route("news/{slug}")]
    [Route("article/{slug}")]
    public async Task<IActionResult> Details(
        string slug)
    {
        var segment = _siteSegment.Current;

        var model =
            await _cacheService.GetOrCreateAsync(
                CacheKeys.ForSegment($"article_{slug}", segment),
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

                    // Related articles follow the article's own segment so a
                    // mobile article never suggests PC/console reading.
                    var relatedSegment =
                        article.Segment.Includes(segment)
                            ? segment
                            : article.Segment;

                    var relatedArticles =
                        await _articleRepository
                            .GetRelatedArticlesAsync(
                                article.CategoryId,
                                article.Id,
                                relatedSegment);
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

        // A reader can always open any article (search engines and shared
        // links must not 404). If the article does not belong to the mode
        // they are browsing, flip the site to the article's own mode and
        // tell them what happened.
        if (!model.Article.Segment.Includes(segment))
        {
            var articleSegment =
                model.Article.Segment.Includes(GameSegment.Mobile)
                    ? GameSegment.Mobile
                    : GameSegment.PcConsole;

            _siteSegment.Set(articleSegment);

            ViewData["SwitchedModeNotice"] =
                articleSegment == GameSegment.Mobile
                    ? "You were switched to Mobile Gaming to read this article."
                    : "You were switched to PC / Console Gaming to read this article.";
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
