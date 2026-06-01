using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class MediaController : BaseAdminController
{
    private readonly IWebHostEnvironment _environment;

    private readonly IUnitOfWork _unitOfWork;

    public MediaController(
        IWebHostEnvironment environment,
        IUnitOfWork unitOfWork)
    {
        _environment = environment;
        _unitOfWork = unitOfWork;
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
        var uploadsFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName =
            $"{Guid.NewGuid()}_{model.File.FileName}";

        var physicalFilePath = Path.Combine(
            uploadsFolder,
            uniqueFileName);

        await using (var stream = new FileStream(
            physicalFilePath,
            FileMode.Create))
        {
            await model.File.CopyToAsync(stream);
        }

        var media = new Media
        {
            FileName = model.File.FileName,

            FilePath = $"/uploads/{uniqueFileName}",

            ContentType = model.File.ContentType,

            FileSize = model.File.Length,

            IsImage = model.File.ContentType
                .StartsWith("image/")
        };

        await _unitOfWork.Media
            .AddAsync(media);

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
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

        return RedirectToAction(nameof(Index));
    }
}