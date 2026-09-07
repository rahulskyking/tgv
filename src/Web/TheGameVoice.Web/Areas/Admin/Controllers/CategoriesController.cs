using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Constants;
using TheGameVoice.Domain.Common.Extensions;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Web.Areas.Admin.ViewModels.Categories;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class CategoriesController
    : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ISlugService _slugService;
    private readonly ICacheService
_cacheService;


    public CategoriesController(
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
        var categories =
            await _unitOfWork.Categories
                .GetAllAsync();

        return View(categories);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var category = new Category
        {
            Name = model.Name,

            Segment = model.Segment,

            Slug = await _slugService
                .GenerateSlugAsync(model.Name)
        };

        await _unitOfWork.Categories
            .AddAsync(category);

        await _unitOfWork.SaveChangesAsync();
        _cacheService.RemoveMany(CacheKeys.AllHomePageKeys());

        return RedirectToAction(nameof(Index));
    }
    /// <summary>
    /// Moves a category between the PC / Console and Mobile sections
    /// straight from the list (categories have no dedicated edit screen).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetSegment(
        Guid id,
        GameSegment segment)
    {
        var category =
            await _unitOfWork.Categories.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        category.Segment =
            segment == GameSegment.None
                ? GameSegment.All
                : segment;

        await _unitOfWork.SaveChangesAsync();

        _cacheService.RemoveMany(CacheKeys.AllHomePageKeys());

        TempData["Success"] =
            $"{category.Name} now appears in: {category.Segment.ToDisplayName()}.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reorder([FromBody] List<Guid> ids)
    {
        var categories =
            (await _unitOfWork.Categories.GetAllAsync())
            .ToDictionary(x => x.Id);

        var order = 1;

        foreach (var id in ids)
        {
            if (categories.TryGetValue(id, out var category))
            {
                category.DisplayOrder = order++;
            }
        }

        await _unitOfWork.SaveChangesAsync();

        _cacheService.RemoveMany(CacheKeys.AllHomePageKeys());

        return Ok();
    }
}