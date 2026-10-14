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
    private readonly ICacheService _cacheService;
    private readonly IGameMetadataProvider _gameMetadataProvider;
    private readonly IStorageService _storageService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GamesController> _logger;

    public GamesController(
        IUnitOfWork unitOfWork,
        ISlugService slugService,
        ICacheService cacheService,
        IGameMetadataProvider gameMetadataProvider,
        IStorageService storageService,
        IHttpClientFactory httpClientFactory,
        ILogger<GamesController> logger)
    {
        _unitOfWork = unitOfWork;
        _slugService = slugService;
        _cacheService = cacheService;
        _gameMetadataProvider = gameMetadataProvider;
        _storageService = storageService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var games =
            await _unitOfWork.Games
                .GetAllAsync();

        return View(games);
    }
    [HttpGet]
    public async Task<IActionResult> SearchIgdb(
        string query,
        CancellationToken cancellationToken)
    {
        query = query?.Trim() ?? string.Empty;

        if (query.Length < 2)
        {
            return BadRequest(new
            {
                message = "Enter at least two characters to search."
            });
        }

        if (query.Length > 100)
        {
            return BadRequest(new { message = "The search is too long." });
        }

        if (!_gameMetadataProvider.IsConfigured)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                message = "IGDB is not configured. Add Igdb__ClientId and Igdb__ClientSecret to the server environment."
            });
        }

        try
        {
            var games = await _gameMetadataProvider
                .SearchAsync(query, cancellationToken);

            return Json(games);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return new EmptyResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "IGDB search failed for {Query}.", query);

            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                message = "IGDB is temporarily unavailable. You can still create the game manually."
            });
        }
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
        if (!model.CoverImageId.HasValue &&
            !string.IsNullOrWhiteSpace(model.IgdbCoverUrl))
        {
            model.CoverImageId = await ImportIgdbImageAsync(
                model.IgdbCoverUrl,
                $"{model.Name}-cover.jpg",
                cancellationToken: HttpContext.RequestAborted);
        }

        if (!model.BannerImageId.HasValue &&
            !string.IsNullOrWhiteSpace(model.IgdbBannerUrl))
        {
            model.BannerImageId = await ImportIgdbImageAsync(
                model.IgdbBannerUrl,
                $"{model.Name}-banner.jpg",
                cancellationToken: HttpContext.RequestAborted);
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

            Segment =
             model.Segment,

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

            DataSource = model.IgdbGameId.HasValue
                ? GameDataSource.Igdb
                : GameDataSource.Manual
        };

        await _unitOfWork.Games
            .AddAsync(game);

        await _unitOfWork.SaveChangesAsync();
        _cacheService.RemoveMany(CacheKeys.AllHomePageKeys());

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

        // Tick the section checkboxes from the saved game.
        model.SetSegment(game.Segment);

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

        game.Segment =
            model.Segment;

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

    private async Task<Guid?> ImportIgdbImageAsync(
        string imageUrl,
        string fileName,
        CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttps ||
            !uri.Host.Equals("images.igdb.com", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Rejected an invalid IGDB image URL.");
            return null;
        }

        try
        {
            fileName = string.Concat(fileName.Select(character =>
                char.IsLetterOrDigit(character) || character is '.' or '-' or '_'
                    ? character
                    : '-'));

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(15);

            using var response = await client.GetAsync(
                uri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
            response.EnsureSuccessStatusCode();

            const long maxBytes = 5 * 1024 * 1024;
            if (response.Content.Headers.ContentLength > maxBytes)
            {
                throw new InvalidOperationException("The IGDB image exceeds 5 MB.");
            }

            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType is not ("image/jpeg" or "image/png" or "image/webp"))
            {
                throw new InvalidOperationException("IGDB returned an unsupported image type.");
            }

            await using var source = await response.Content
                .ReadAsStreamAsync(cancellationToken);
            await using var buffer = new MemoryStream();
            await source.CopyToAsync(buffer, cancellationToken);

            if (buffer.Length > maxBytes)
            {
                throw new InvalidOperationException("The IGDB image exceeds 5 MB.");
            }

            buffer.Position = 0;
            var storedPath = await _storageService.UploadAsync(
                buffer,
                fileName,
                contentType,
                cancellationToken);

            var media = new Media
            {
                FileName = fileName,
                FilePath = storedPath,
                ContentType = contentType,
                FileSize = buffer.Length,
                IsImage = true,
                AltText = Path.GetFileNameWithoutExtension(fileName),
                Credit = "IGDB"
            };

            await _unitOfWork.Media.AddAsync(media);
            return media.Id;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not import image from IGDB.");
            TempData["Warning"] =
                "The game was imported, but one or more IGDB images could not be saved.";
            return null;
        }
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