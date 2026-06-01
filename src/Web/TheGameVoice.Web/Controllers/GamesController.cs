using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Web.ViewModels.Games;

namespace TheGameVoice.Web.Controllers;

public class GamesController : Controller
{
    private readonly IGameRepository
        _gameRepository;



    public GamesController(
        IGameRepository gameRepository)
    {
        _gameRepository =
            gameRepository;
    }

    [Route("game/{slug}")]
    public async Task<IActionResult> Details(
        string slug)
    {
        var game =
            await _gameRepository
                .GetBySlugWithArticlesAsync(slug);

        if (game == null)
        {
            return NotFound();
        }

        var model =
            new GameDetailsViewModel
            {
                Game = game
            };
        ViewData["CanonicalUrl"] =
    Url.Action(
        "Details",
        "Games",
        new { slug = game.Slug },
        Request.Scheme);
        return View(model);
    }
}