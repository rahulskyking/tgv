using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Web.ViewModels.Games;

namespace TheGameVoice.Web.Controllers;

public class GamesController : Controller
{
    private readonly IGameRepository
        _gameRepository;
    private readonly ICacheService
    _cacheService;


    public GamesController(
        IGameRepository gameRepository, ICacheService cacheService)
    {
        _gameRepository =
            gameRepository;
        _cacheService = cacheService;
    }

    [Route("game/{slug}")]
    public async Task<IActionResult> Details(
     string slug)
    {
        var model =
            await _cacheService.GetOrCreateAsync(
                $"{CacheKeys.Games}_{slug}",
                async () =>
                {
                    var game =
                        await _gameRepository
                            .GetBySlugWithArticlesAsync(slug);

                    if (game == null)
                    {
                        return null!;
                    }

                    return new GameDetailsViewModel
                    {
                        Game = game
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
                "Games",
                new { slug = model.Game.Slug },
                Request.Scheme);

        return View(model);
    }

}