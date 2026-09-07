using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Web.Services;
using TheGameVoice.Web.ViewModels.Categories;

namespace TheGameVoice.Web.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoryRepository
        _categoryRepository;

    private readonly IArticleRepository
        _articleRepository;
    private readonly ICacheService
_cacheService;

    private readonly ISiteSegmentAccessor _siteSegment;

    public CategoriesController(
        ICategoryRepository categoryRepository,
        IArticleRepository articleRepository,
        ICacheService cacheService,
        ISiteSegmentAccessor siteSegment)
    {
        _siteSegment = siteSegment;
        _categoryRepository =
            categoryRepository;

        _articleRepository =
            articleRepository;
        _cacheService = cacheService;
    }
    [Route("category/{slug}")]
    public async Task<IActionResult> Details(
        string slug)
    {
        // Listing pages differ per site mode, so cache per mode.
        var segment = _siteSegment.Current;

        var model =
            await _cacheService.GetOrCreateAsync(
                CacheKeys.ForSegment(
                    $"{CacheKeys.Categories}_{slug}",
                    segment),
                async () =>
                {
                    var category =
                        await _categoryRepository
                            .GetBySlugAsync(slug);

                    if (category == null)
                    {
                        return null!;
                    }

                    var articles =
                        await _articleRepository
                            .GetPublishedByCategoryAsync(
                                category.Id,
                                segment);

                    return new CategoryDetailsViewModel
                    {
                        Category = category,
                        Articles = articles
                    };
                },
                CacheDurations.Short);

        if (model == null)
        {
            return NotFound();
        }

        ViewData["CanonicalUrl"] =
            Url.Action(
                "Details",
                "Categories",
                new { slug = model.Category.Slug },
                Request.Scheme);

        return View(model);
    }
}