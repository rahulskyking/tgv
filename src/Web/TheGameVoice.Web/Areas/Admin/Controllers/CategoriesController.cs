using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Web.Areas.Admin.ViewModels.Categories;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class CategoriesController
    : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ISlugService _slugService;



    public CategoriesController(
        IUnitOfWork unitOfWork,
        ISlugService slugService)
    {
        _unitOfWork = unitOfWork;
        _slugService = slugService;
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

            Slug = await _slugService
                .GenerateSlugAsync(model.Name)
        };

        await _unitOfWork.Categories
            .AddAsync(category);

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}