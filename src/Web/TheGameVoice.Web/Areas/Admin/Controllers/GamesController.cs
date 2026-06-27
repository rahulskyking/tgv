using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Web.Areas.Admin.ViewModels.Games;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class GamesController : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISlugService _slugService;
    private readonly ICacheService
_cacheService;
    public GamesController(
        IUnitOfWork unitOfWork,
        ISlugService slugService,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _slugService = slugService;
        _cacheService = cacheService;
    }

    public async Task<IActionResult> Index()
    {
        var games =
            await _unitOfWork.Games
                .GetAllAsync();

        return View(games);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model =
            new CreateGameViewModel();

        model.MediaItems =
            (await _unitOfWork.Media.GetAllAsync())
            .Select(x => new MediaPickerItemViewModel
            {
                Id = x.Id,
                FileName = x.FileName,
                FilePath = x.FilePath
            })
            .ToList();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
    CreateGameViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.MediaItems =
                (await _unitOfWork.Media.GetAllAsync())
                .Select(x => new MediaPickerItemViewModel
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    FilePath = x.FilePath
                })
                .ToList();

            return View(model);
        }
        var existingGame =
    (await _unitOfWork.Games.GetAllAsync())
    .FirstOrDefault(x =>
        x.Name.ToLower().Trim() ==
        model.Name.ToLower().Trim());

        if (existingGame != null)
        {
            ModelState.AddModelError(
                nameof(model.Name),
                "A game with this name already exists.");

            model.MediaItems =
                (await _unitOfWork.Media.GetAllAsync())
                .Select(x => new MediaPickerItemViewModel
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    FilePath = x.FilePath
                })
                .ToList();

            return View(model);
        }
        var game = new Game
        {
            Name = model.Name,

            Slug =
             await _slugService.GenerateSlugAsync(
                 model.Name),

            Summary = model.Summary ?? string.Empty,

            Description =
             model.Description ?? string.Empty,

            ReleaseDate =
    model.ReleaseDate.HasValue
        ? DateTime.SpecifyKind(
            model.ReleaseDate.Value,
            DateTimeKind.Utc)
        : null,

            Developer =
             model.Developer,

            Publisher =
             model.Publisher,

            Platforms =
             model.Platforms,

            Genres =
             model.Genres,

            OfficialWebsite =
             model.OfficialWebsite,

            SteamUrl =
             model.SteamUrl,

            SteamAppId =
             model.SteamAppId,

            CoverImageId =
             model.CoverImageId,

            BannerImageId =
             model.BannerImageId,

            DataSource =
             GameDataSource.Manual
        };

        await _unitOfWork.Games
            .AddAsync(game);

        await _unitOfWork.SaveChangesAsync();
        _cacheService.RemoveMany(CacheKeys.HomePage);

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

                Description = game.Description,

                ReleaseDate = game.ReleaseDate,

                Developer = game.Developer,

                Publisher = game.Publisher,

                Platforms = game.Platforms,

                Genres = game.Genres,

                OfficialWebsite = game.OfficialWebsite,

                SteamUrl = game.SteamUrl,

                SteamAppId = game.SteamAppId,

                CoverImageId = game.CoverImageId,

                BannerImageId = game.BannerImageId,

                CoverImagePath = game.CoverImage?.FilePath,

                BannerImagePath = game.BannerImage?.FilePath,

                MediaItems =
                    (await _unitOfWork.Media.GetAllAsync())
                    .Select(x => new MediaPickerItemViewModel
                    {
                        Id = x.Id,
                        FileName = x.FileName,
                        FilePath = x.FilePath
                    })
                    .ToList()
            };

        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(
    EditGameViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.MediaItems =
                (await _unitOfWork.Media.GetAllAsync())
                .Select(x => new MediaPickerItemViewModel
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    FilePath = x.FilePath
                })
                .ToList();

            return View(model);
        }
        var existingGame =
    (await _unitOfWork.Games.GetAllAsync())
    .FirstOrDefault(x =>
        x.Id != model.Id &&
        x.Name.ToLower().Trim() ==
        model.Name.ToLower().Trim());

        if (existingGame != null)
        {
            ModelState.AddModelError(
                nameof(model.Name),
                "A game with this name already exists.");

            model.MediaItems =
                (await _unitOfWork.Media.GetAllAsync())
                .Select(x => new MediaPickerItemViewModel
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    FilePath = x.FilePath
                })
                .ToList();

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
                .GenerateSlugAsync(
                    model.Name);

        game.Summary =
            model.Summary ?? string.Empty;

        game.Description =
            model.Description ?? string.Empty;

        game.ReleaseDate =
     model.ReleaseDate.HasValue
         ? DateTime.SpecifyKind(
             model.ReleaseDate.Value,
             DateTimeKind.Utc)
         : null;

        game.Developer =
            model.Developer;

        game.Publisher =
            model.Publisher;

        game.Platforms =
            model.Platforms;

        game.Genres =
            model.Genres;

        game.OfficialWebsite =
            model.OfficialWebsite;

        game.SteamUrl =
            model.SteamUrl;

        game.SteamAppId =
            model.SteamAppId;

        game.CoverImageId =
            model.CoverImageId;

        game.BannerImageId =
            model.BannerImageId;

        _unitOfWork.Games
            .Update(game);

        await _unitOfWork
            .SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    #region Quick Create
    [HttpPost]
    public async Task<IActionResult> QuickCreate(
    [FromBody] QuickCreateGameRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest();
        }

        var existingGames =
            await _unitOfWork.Games.GetAllAsync();

        if (existingGames.Any(x =>
            x.Name.ToLower() ==
            request.Name.ToLower()))
        {
            return BadRequest(
                "Game already exists.");
        }

        var game = new Game
        {
            Name =
         request.Name.Trim(),

            Slug =
         await _slugService.GenerateSlugAsync(
             request.Name),

            Summary = string.Empty,

            Description = string.Empty,

            DataSource =
         GameDataSource.Manual
        };

        await _unitOfWork.Games
            .AddAsync(game);

        await _unitOfWork
            .SaveChangesAsync();

        return Json(
            new
            {
                id = game.Id,
                name = game.Name
            });
    }
    #endregion


    #region Delete Game
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
    Guid id)
    {
        var game =
            await _unitOfWork.Games
                .GetByIdAsync(id);

        if (game is null)
        {
            return NotFound();
        }

        _unitOfWork.Games
            .Remove(game);

        await _unitOfWork
            .SaveChangesAsync();

        return RedirectToAction(
            nameof(Index));
    }
    #endregion
}