using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Infrastructure.Persistence.UnitOfWork;
using TheGameVoice.Web.ViewModels.Tags;

namespace TheGameVoice.Web.Controllers;

public class TagsController : Controller
{
    private readonly ITagRepository
        _tagRepository;

    private readonly IArticleRepository
        _articleRepository;
    private readonly ICacheService
_cacheService;

    public TagsController(
        ITagRepository tagRepository,
        IArticleRepository articleRepository,
        ICacheService cacheService)
    {
        _tagRepository =
            tagRepository;

        _articleRepository =
            articleRepository;
        _cacheService = cacheService;
    }


    [Route("tag/{slug}")]
    public async Task<IActionResult> Details(
        string slug)
    {
        var model =
            await _cacheService.GetOrCreateAsync(
                $"{CacheKeys.Tags}_{slug}",
                async () =>
                {
                    var tag =
                        await _tagRepository
                            .GetBySlugAsync(slug);

                    if (tag == null)
                    {
                        return null!;
                    }

                    var articles =
                        await _articleRepository
                            .GetPublishedByTagAsync(slug);

                    return new TagDetailsViewModel
                    {
                        Tag = tag,
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
                "Tags",
                new { slug = model.Tag.Slug },
                Request.Scheme);

        return View(model);
    }



}