using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Common.Extensions;
using TheGameVoice.Web.Services;
using TheGameVoice.Web.ViewModels.Home;

namespace TheGameVoice.Web.Controllers;

public class HomeController : Controller
{
    private readonly IArticleRepository
        _articleRepository;
    private readonly IGameRepository
    _gameRepository;
    private readonly ICacheService
    _cacheService;

    private readonly IArticleViewRepository _articleViewRepository;

    private readonly ISiteSegmentAccessor _siteSegment;

    public HomeController(
        IArticleRepository articleRepository,
        IGameRepository gameRepository,
        ICacheService cacheService,
        IArticleViewRepository articleViewRepository,
        ISiteSegmentAccessor siteSegment)
    {
        _articleRepository =
            articleRepository;

        _gameRepository =
            gameRepository;
        _cacheService = cacheService;
        _articleViewRepository = articleViewRepository;
        _siteSegment = siteSegment;
    }

    public async Task<IActionResult> Index()
    {
        // The homepage is rendered per site mode, so the cache key must be
        // scoped to it — otherwise mobile content leaks into the PC cache.
        var segment = _siteSegment.Current;

        var model =
            await _cacheService.GetOrCreateAsync(
                CacheKeys.ForSegment(CacheKeys.HomePage, segment),
                async () =>
                {
                    var latestNews =
                        await _articleRepository
                            .GetPublishedAsync(segment);

                    var games =
                        (await _gameRepository
                            .GetAllAsync())
                        .Where(x => x.Segment.Includes(segment))
                        .ToList();

                    var reviews =
                        latestNews
                            .Where(x =>
                                x.Category != null
                                &&
                                x.Category.Name == "Reviews")
                            .ToList();
                    var trendingArticles =
                                await _articleViewRepository
                                    .GetTrendingArticlesAsync(5, segment);
                    return new HomePageViewModel
                    {
                        Segment = segment,

                        HeroArticle =
                            latestNews.FirstOrDefault(),

                        LatestNews =
                            latestNews.Take(9).ToList(),

                        FeaturedReview =
                            reviews.FirstOrDefault(),

                        Reviews =
                            reviews
                                .Skip(1)
                                .Take(4)
                                .ToList(),

                        TrendingGames =
                            games.Take(6).ToList(),

                        LatestReviews =
                            reviews
                                .Take(4)
                                .ToList(),
                        TrendingArticles = trendingArticles
                    };
                },
                TimeSpan.FromMinutes(5));

        return View(model);
    }
}
