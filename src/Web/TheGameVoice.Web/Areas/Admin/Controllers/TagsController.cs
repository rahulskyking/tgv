using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Web.Areas.Admin.ViewModels.Tags;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class TagsController : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ISlugService _slugService;

    public TagsController(
        IUnitOfWork unitOfWork,
        ISlugService slugService)
    {
        _unitOfWork = unitOfWork;
        _slugService = slugService;
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

        return RedirectToAction(nameof(Index));
    }
}