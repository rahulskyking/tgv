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
 
    public async Task<IActionResult> Upload(
    UploadMediaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.File == null || model.File.Length == 0)
        {
            ModelState.AddModelError(
                "File",
                "Please select a valid file.");

            return View(model);
        }

        var allowedTypes = new[]
        {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

        if (!allowedTypes.Contains(
            model.File.ContentType))
        {
            ModelState.AddModelError(
                "File",
                "Only JPG, PNG, and WEBP images are allowed.");

            return View(model);
        }

        if (model.File.Length > 5 * 1024 * 1024)
        {
            ModelState.AddModelError(
                "File",
                "Maximum file size is 5MB.");

            return View(model);
        }

        try
        {
            Console.WriteLine("Upload started");

            await using var stream = model.File.OpenReadStream();

            var filePath = await _storageService.UploadAsync(
                stream, model.File.FileName, model.File.ContentType);

            var media = new Media
            {
                FileName = model.File.FileName,
                FilePath = filePath,
                ContentType = model.File.ContentType,
                FileSize = model.File.Length,
                IsImage = model.File.ContentType.StartsWith("image/")
            };

            await _unitOfWork.Media.AddAsync(media);     // ← Only once
            await _unitOfWork.SaveChangesAsync();_cacheService.RemoveMany(CacheKeys.HomePage);        // ← Only once

            Console.WriteLine($"Media saved with ID: {media.Id}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            // Consider logging + showing a friendly error instead of re-throwing
            throw;
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