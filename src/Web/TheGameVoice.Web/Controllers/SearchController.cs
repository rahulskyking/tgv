using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Web.Services;
using TheGameVoice.Web.ViewModels.Search;

namespace TheGameVoice.Web.Controllers;

public class SearchController : Controller
{
    private readonly IArticleRepository
        _articleRepository;

    private readonly ISiteSegmentAccessor _siteSegment;

    public SearchController(
        IArticleRepository articleRepository,
        ISiteSegmentAccessor siteSegment)
    {
        _articleRepository =
            articleRepository;

        _siteSegment = siteSegment;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string query)
    {
        var articles =
            new List<TheGameVoice.Domain.Entities.Article>();

        if (!string.IsNullOrWhiteSpace(query))
        {
            articles =
                (await _articleRepository
                    .SearchAsync(
                        query,
                        _siteSegment.Current))
                .ToList();
        }

        var model =
            new SearchResultsViewModel
            {
                Query = query,
                Articles = articles
            };

        return View(model);
    }
}