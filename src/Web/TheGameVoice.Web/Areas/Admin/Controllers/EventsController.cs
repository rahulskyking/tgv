using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Identity;
using TheGameVoice.Web.Areas.Admin.ViewModels.Events;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

[Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
[AutoValidateAntiforgeryToken]
public class EventsController : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;
    public EventsController(IUnitOfWork unitOfWork, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> Index() => View(await _unitOfWork.Events.GetAllAsync());

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new EventFormViewModel();
        await PopulateMedia(model);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(EventFormViewModel model)
    {
        await ValidateReferences(model);
        if (!ModelState.IsValid)
        {
            await PopulateMedia(model);
            return View(model);
        }

        var gamingEvent = new GamingEvent { Slug = model.Slug };
        Apply(model, gamingEvent);
        await _unitOfWork.Events.AddAsync(gamingEvent);
        if (!await Save(model))
        {
            await PopulateMedia(model);
            return View(model);
        }
        TempData["Success"] = "Event created. Assign articles to it from the article editor.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _unitOfWork.Events.GetByIdAsync(id);
        if (item is null) return NotFound();
        var model = new EventFormViewModel
        {
            Id = item.Id, Name = item.Name, Slug = item.Slug, Description = item.Description,
            BannerImageId = item.BannerImageId, BannerAltText = item.BannerAltText,
            StartDate = item.StartDate, EndDate = item.EndDate, Location = item.Location,
            OfficialWebsite = item.OfficialWebsite, IsActive = item.IsActive,
            ShowInNavigation = item.ShowInNavigation, DisplayOrder = item.DisplayOrder
        };
        await PopulateMedia(model);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, EventFormViewModel model)
    {
        if (model.Id != id) return BadRequest();
        var item = await _unitOfWork.Events.GetByIdAsync(id);
        if (item is null) return NotFound();
        if (model.Slug != item.Slug)
            ModelState.AddModelError(nameof(model.Slug), "The event URL cannot be changed after creation.");
        await ValidateReferences(model, id);
        if (!ModelState.IsValid)
        {
            await PopulateMedia(model);
            return View(model);
        }
        Apply(model, item);
        if (!await Save(model))
        {
            await PopulateMedia(model);
            return View(model);
        }
        await InvalidateArticles(item.Id);
        TempData["Success"] = "Event updated. Archiving never removes articles or their assignments.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SetActive(Guid id, bool isActive)
    {
        var item = await _unitOfWork.Events.GetByIdAsync(id);
        if (item is null) return NotFound();
        item.IsActive = isActive;
        await _unitOfWork.SaveChangesAsync();
        await InvalidateArticles(item.Id);
        TempData["Success"] = isActive ? "Event reactivated." : "Event archived. Its page and articles remain public.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateReferences(EventFormViewModel model, Guid? id = null)
    {
        if (!string.IsNullOrWhiteSpace(model.Slug)
            && await _unitOfWork.Events.SlugExistsAsync(model.Slug, id))
            ModelState.AddModelError(nameof(model.Slug), "This event URL is already in use.");
        if (model.BannerImageId.HasValue)
        {
            var media = await _unitOfWork.Media.GetByIdAsync(model.BannerImageId.Value);
            if (media is null || !media.IsImage)
                ModelState.AddModelError(nameof(model.BannerImageId), "Select an image from the media library.");
        }
    }

    private async Task PopulateMedia(EventFormViewModel model) =>
        model.BannerImages = (await _unitOfWork.Media.GetAllAsync()).Where(x => x.IsImage)
            .Select(x => new MediaPickerItemViewModel { Id = x.Id, FileName = x.FileName, FilePath = x.FilePath }).ToList();

    private static void Apply(EventFormViewModel model, GamingEvent item)
    {
        item.Name = model.Name.Trim();
        item.Description = model.Description?.Trim();
        item.BannerImageId = model.BannerImageId;
        item.BannerAltText = model.BannerAltText?.Trim();
        item.StartDate = model.StartDate;
        item.EndDate = model.EndDate;
        item.Location = model.Location?.Trim();
        item.OfficialWebsite = string.IsNullOrWhiteSpace(model.OfficialWebsite) ? null : model.OfficialWebsite.Trim();
        item.IsActive = model.IsActive;
        item.ShowInNavigation = model.ShowInNavigation;
        item.DisplayOrder = model.DisplayOrder;
    }

    private async Task<bool> Save(EventFormViewModel model)
    {
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
        {
            ModelState.AddModelError(nameof(model.Slug), "This event URL was just taken. Choose another.");
            return false;
        }
    }

    private async Task InvalidateArticles(Guid eventId)
    {
        foreach (var slug in await _unitOfWork.Events.GetArticleSlugsAsync(eventId))
        {
            _cache.Remove(CacheKeys.ForSegment($"article_{slug}", GameSegment.PcConsole));
            _cache.Remove(CacheKeys.ForSegment($"article_{slug}", GameSegment.Mobile));
        }
    }
}
