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
using TheGameVoice.Web.Areas.Admin.ViewModels.Tags;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class TagsController : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISlugService _slugService;
    private readonly ICacheService _cacheService;

    public TagsController(IUnitOfWork unitOfWork, ISlugService slugService, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _slugService = slugService;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, string usage = "all", string sort = "name", int page = 1)
    {
        search = (search ?? string.Empty).Trim();
        if (search.Length > 100) search = search[..100];
        if (usage is not ("used" or "unused")) usage = "all";
        if (sort is not ("newest" or "most-used" or "name-desc")) sort = "name";
        return View(new TagIndexViewModel
        {
            Search = search, Usage = usage, Sort = sort,
            Result = await _unitOfWork.Tags.GetAdminPageAsync(search, usage, sort, page)
        });
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateTagViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTagViewModel model)
    {
        await ValidateName(model);
        if (!ModelState.IsValid) return View(model);

        var tag = new Tag { Name = model.Name, Slug = await GenerateTagSlug(model.Name) };
        await _unitOfWork.Tags.AddAsync(tag);
        if (!await SaveTag()) return View(model);
        InvalidateTag(tag.Slug);
        TempData["Success"] = $"Tag '{tag.Name}' created. You can now assign it to articles.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id);
        if (tag is null) return NotFound();
        return View(new EditTagViewModel { Id = tag.Id, Name = tag.Name, Slug = tag.Slug });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditTagViewModel model)
    {
        if (id != model.Id) return BadRequest();
        var tag = await _unitOfWork.Tags.GetByIdAsync(id);
        if (tag is null) return NotFound();
        // Names are editable; existing public URLs and assignments never change.
        model.Slug = tag.Slug;
        await ValidateName(model, id);
        if (!ModelState.IsValid) return View(model);

        tag.Name = model.Name;
        if (!await SaveTag()) return View(model);
        InvalidateTag(tag.Slug);
        foreach (var slug in await _unitOfWork.Tags.GetArticleSlugsAsync(id))
        {
            foreach (var segment in new[] { GameSegment.PcConsole, GameSegment.Mobile })
                _cacheService.Remove(CacheKeys.ForSegment($"article_{slug}", segment));
        }
        TempData["Success"] = "Tag name updated. Its URL and article assignments are unchanged.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string? search, string usage = "all", string sort = "name", int page = 1)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id);
        if (tag is null) return NotFound();
        if (await _unitOfWork.Tags.IsInUseAsync(id))
        {
            TempData["Error"] = $"Cannot delete '{tag.Name}': it is assigned to articles. Remove those assignments first.";
            return RedirectToAction(nameof(Index), new { search, usage, sort, page });
        }
        _unitOfWork.Tags.Remove(tag);
        await _unitOfWork.SaveChangesAsync();
        InvalidateTag(tag.Slug);
        TempData["Success"] = $"Tag '{tag.Name}' deleted. No articles were changed.";
        return RedirectToAction(nameof(Index), new { search, usage, sort, page });
    }

    // Preserve the article editor's existing JSON endpoint and response shape.
    [Authorize(Roles = $"{Roles.Author},{Roles.Editor},{Roles.Admin},{Roles.SuperAdmin}")]
    [HttpPost]
    public async Task<IActionResult> QuickCreate([FromBody] QuickCreateTagRequest? request)
    {
        var model = new CreateTagViewModel { Name = request?.Name?.Trim() ?? string.Empty };
        await ValidateName(model);
        if (!ModelState.IsValid) return BadRequest("Use a unique tag name between 1 and 100 characters.");
        var tag = new Tag { Name = model.Name, Slug = await GenerateTagSlug(model.Name) };
        await _unitOfWork.Tags.AddAsync(tag);
        if (!await SaveTag()) return BadRequest("This tag was just created. Refresh the tag list and try again.");
        InvalidateTag(tag.Slug);
        return Json(new { id = tag.Id, name = tag.Name });
    }

    private async Task ValidateName(CreateTagViewModel model, Guid? id = null)
    {
        model.Name = (model.Name ?? string.Empty).Trim();
        if (model.Name.Length is < 1 or > 100)
            ModelState.AddModelError(nameof(model.Name), "Enter a tag name between 1 and 100 characters.");
        else if (await _unitOfWork.Tags.NameExistsAsync(model.Name, id))
            ModelState.AddModelError(nameof(model.Name), "A tag with this name already exists. Find it in the tag library instead.");
    }

    private async Task<string> GenerateTagSlug(string name)
    {
        var baseSlug = await _slugService.GenerateSlugAsync(name);
        if (string.IsNullOrWhiteSpace(baseSlug)) baseSlug = "tag";
        var slug = baseSlug;
        var suffix = 2;
        while (await _unitOfWork.Tags.SlugExistsAsync(slug)) slug = $"{baseSlug}-{suffix++}";
        return slug;
    }

    private async Task<bool> SaveTag()
    {
        try { await _unitOfWork.SaveChangesAsync(); return true; }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
        {
            ModelState.AddModelError("Name", "This tag URL was just taken. Please try saving again.");
            return false;
        }
    }

    private void InvalidateTag(string slug)
    {
        _cacheService.RemoveMany(CacheKeys.AllHomePageKeys());
        foreach (var segment in new[] { GameSegment.PcConsole, GameSegment.Mobile })
            _cacheService.Remove(CacheKeys.ForSegment($"{CacheKeys.Tags}_{slug}", segment));
    }
}
