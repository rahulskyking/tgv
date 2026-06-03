using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Web.ViewModels.Home;

namespace TheGameVoice.Web.Controllers;

public class HomeController : Controller
{
    private readonly IArticleRepository
        _articleRepository;
    private readonly IGameRepository
    _gameRepository;
    public HomeController(
        IArticleRepository articleRepository,
        IGameRepository gameRepository)
    {
        _articleRepository =
            articleRepository;

        _gameRepository =
            gameRepository;
    }

    public async Task<IActionResult> Index()
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

        var model = new HomePageViewModel
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
        .ToList()

        };

        return View(model);
    }
}