using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Infrastructure.Identity;
using TheGameVoice.Web.Areas.Admin.ViewModels.Tags;
using TheGameVoice.Web.ViewModels.Tags;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class TagsController : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ISlugService _slugService;
    private readonly ICacheService
_cacheService;
    public TagsController(
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
        var tags =
            await _unitOfWork.Tags
                .GetAllAsync();

        return View(tags);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTagViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var tag = new Tag
        {
            Name = model.Name,

            Slug = await _slugService
                .GenerateSlugAsync(model.Name)
        };

        await _unitOfWork.Tags
            .AddAsync(tag);

        await _unitOfWork.SaveChangesAsync();
        _cacheService.RemoveMany(CacheKeys.HomePage);

        return RedirectToAction(nameof(Index));
    }

    #region Quick Tag add
    [Authorize(
Roles =
$"{Roles.Author}," +
$"{Roles.Editor}," +
$"{Roles.Admin}," +
$"{Roles.SuperAdmin}")]
    [HttpPost]

    public async Task<IActionResult> QuickCreate(
    [FromBody] QuickCreateTagRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest();
        }

        var existingTags =
            await _unitOfWork.Tags.GetAllAsync();

        if (existingTags.Any(x =>
            x.Name.ToLower() ==
            request.Name.ToLower()))
        {
            return BadRequest(
                "Tag already exists.");
        }

        var tag = new Tag
        {
            Name = request.Name.Trim(),

            Slug =
    await _slugService
        .GenerateSlugAsync(
            request.Name)
        };

        await _unitOfWork.Tags
            .AddAsync(tag);

        await _unitOfWork
            .SaveChangesAsync();

        return Json(
            new
            {
                id = tag.Id,
                name = tag.Name
            });
    }
    #endregion
}