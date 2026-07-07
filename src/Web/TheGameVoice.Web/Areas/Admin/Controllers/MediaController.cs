using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class MediaController : BaseAdminController
{
    private readonly IStorageService _storageService;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService
_cacheService;
    public MediaController(
        IStorageService storageService,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _storageService = storageService;

        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<IActionResult> Index()
    {
        var mediaItems =
            await _unitOfWork.Media.GetAllAsync();

        return View(mediaItems);
    }

    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(
        UploadMediaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.Files == null || !model.Files.Any())
        {
            ModelState.AddModelError(
                "Files",
                "Please select at least one image.");

            return View(model);
        }

        var allowedTypes = new[]
        {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

        foreach (var file in model.Files)
        {
            if (file == null || file.Length == 0)
                continue;

            if (!allowedTypes.Contains(file.ContentType))
            {
                ModelState.AddModelError(
                    "Files",
                    $"{file.FileName} is not a supported image.");

                return View(model);
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(
                    "Files",
                    $"{file.FileName} exceeds the 5MB limit.");

                return View(model);
            }
        }

        try
        {
            foreach (var file in model.Files)
            {
                await using var stream = file.OpenReadStream();

                var filePath = await _storageService.UploadAsync(
                    stream,
                    file.FileName,
                    file.ContentType);

                var media = new Media
                {
                    FileName = file.FileName,
                    FilePath = filePath,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    IsImage = file.ContentType.StartsWith("image/")
                };

                await _unitOfWork.Media.AddAsync(media);
            }

            await _unitOfWork.SaveChangesAsync();

            _cacheService.RemoveMany(CacheKeys.HomePage);

            TempData["Success"] =
                $"{model.Files.Count} image(s) uploaded successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            ModelState.AddModelError(
                "",
                "An error occurred while uploading the images.");

            return View(model);
        }
    }
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var media =
            await _unitOfWork.Media.GetByIdAsync(id);

        if (media is null)
        {
            return NotFound();
        }

        var model =
            new EditMediaViewModel
            {
                Id = media.Id,

                FileName = media.FileName,

                FilePath = media.FilePath,

                AltText = media.AltText,

                Caption = media.Caption,

                Credit = media.Credit
            };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    EditMediaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var media =
            await _unitOfWork.Media
                .GetByIdAsync(model.Id);

        if (media is null)
        {
            return NotFound();
        }

        media.AltText = model.AltText;

        media.Caption = model.Caption;

        media.Credit = model.Credit;

        _unitOfWork.Media.Update(media);

        await _unitOfWork.SaveChangesAsync();
        _cacheService.RemoveMany(CacheKeys.HomePage);

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
    Guid id)
    {
        var media =
            await _unitOfWork.Media
                .GetByIdAsync(id);

        if (media is null)
        {
            return NotFound();
        }

        await _storageService.DeleteAsync(
            media.FilePath);

        _unitOfWork.Media.Remove(media);

        await _unitOfWork.SaveChangesAsync();_cacheService.RemoveMany(CacheKeys.HomePage);

        return RedirectToAction(
            nameof(Index));
    }
}