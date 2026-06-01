using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Web.Areas.Admin.ViewModels.Games;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class GamesController : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISlugService _slugService;
    public GamesController(
        IUnitOfWork unitOfWork,
        ISlugService slugService)
    {
        _unitOfWork = unitOfWork;
        _slugService = slugService;
    }

    public async Task<IActionResult> Index()
    {
        var games =
            await _unitOfWork.Games
                .GetAllAsync();

        return View(games);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(
    CreateGameViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var game = new Game
        {
            Name = model.Name,

            Slug = await _slugService
         .GenerateSlugAsync(model.Name),

            Summary = model.Summary,

            ReleaseDate = model.ReleaseDate
        };

        await _unitOfWork.Games
            .AddAsync(game);

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var game =
            await _unitOfWork.Games
                .GetByIdAsync(id);

        if (game == null)
        {
            return NotFound();
        }

        var model =
            new EditGameViewModel
            {
                Id = game.Id,
                Name = game.Name,
                Summary = game.Summary,
                ReleaseDate = game.ReleaseDate
            };

        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(
    EditGameViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var game =
            await _unitOfWork.Games
                .GetByIdAsync(model.Id);

        if (game == null)
        {
            return NotFound();
        }

        game.Name = model.Name;

        game.Slug =
            await _slugService
                .GenerateSlugAsync(model.Name);

        game.Summary = model.Summary;

        game.ReleaseDate =
            model.ReleaseDate;

        _unitOfWork.Games
            .Update(game);

        await _unitOfWork
            .SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}