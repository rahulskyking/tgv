using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Infrastructure.Persistence.UnitOfWork;
using TheGameVoice.Web.ViewModels.Tags;

namespace TheGameVoice.Web.Controllers;

public class TagsController : Controller
{
    private readonly ITagRepository
        _tagRepository;

    private readonly IArticleRepository
        _articleRepository;

    public TagsController(
        ITagRepository tagRepository,
        IArticleRepository articleRepository)
    {
        _tagRepository =
            tagRepository;

        _articleRepository =
            articleRepository;
    }

    [Route("tag/{slug}")]
    public async Task<IActionResult> Details(
        string slug)
    {
        var tag =
            await _tagRepository
                .GetBySlugAsync(slug);

        if (tag == null)
        {
            return NotFound();
        }

        var articles =
            await _articleRepository
                .GetPublishedByTagAsync(slug);

        var model =
            new TagDetailsViewModel
            {
                Tag = tag,
                Articles = articles
            };
        ViewData["CanonicalUrl"] =
    Url.Action(
        "Details",
        "Tags",
        new { slug = tag.Slug },
        Request.Scheme);

        return View(model);
    }


   
}