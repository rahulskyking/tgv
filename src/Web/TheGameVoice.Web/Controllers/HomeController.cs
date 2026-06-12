using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
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
    public HomeController(
        IArticleRepository articleRepository,
        IGameRepository gameRepository,
        ICacheService cacheService,
        IArticleViewRepository articleViewRepository)
    {
        _articleRepository =
            articleRepository;

        _gameRepository =
            gameRepository;
        _cacheService = cacheService;
        _articleViewRepository = articleViewRepository;
    }

    public async Task<IActionResult> Index()
    {
        var model =
            await _cacheService.GetOrCreateAsync(
               CacheKeys.HomePage,
                async () =>
                {
                    var latestNews =
                        await _articleRepository
                            .GetPublishedAsync();

                    var games =
                        await _gameRepository
                            .GetAllAsync();

                    var reviews =
                        latestNews
                            .Where(x =>
                                x.Category != null
                                &&
                                x.Category.Name == "Reviews")
                            .ToList();
                    var trendingArticles =
                                await _articleViewRepository
                                    .GetTrendingArticlesAsync(5);
                    return new HomePageViewModel
                    {
                        HeroArticle =
                            latestNews.FirstOrDefault(),

                        LatestNews =
                            latestNews.Take(6).ToList(),

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