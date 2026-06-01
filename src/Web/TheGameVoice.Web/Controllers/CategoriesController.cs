using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Web.ViewModels.Categories;

namespace TheGameVoice.Web.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoryRepository
        _categoryRepository;

    private readonly IArticleRepository
        _articleRepository;

    public CategoriesController(
        ICategoryRepository categoryRepository,
        IArticleRepository articleRepository)
    {
        _categoryRepository =
            categoryRepository;

        _articleRepository =
            articleRepository;
    }

    [Route("category/{slug}")]
    public async Task<IActionResult> Details(
        string slug)
    {
        var category =
            await _categoryRepository
                .GetBySlugAsync(slug);

        if (category == null)
        {
            return NotFound();
        }

        var articles =
            await _articleRepository
                .GetPublishedByCategoryAsync(
                    category.Id);

        var model =
            new CategoryDetailsViewModel
            {
                Category = category,
                Articles = articles
            };
        ViewData["CanonicalUrl"] =
    Url.Action(
        "Details",
        "Categories",
        new { slug = category.Slug },
        Request.Scheme);
        return View(model);
    }
}