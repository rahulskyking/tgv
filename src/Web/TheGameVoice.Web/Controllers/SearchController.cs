using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Web.ViewModels.Search;

namespace TheGameVoice.Web.Controllers;

public class SearchController : Controller
{
    private readonly IArticleRepository
        _articleRepository;

    public SearchController(
        IArticleRepository articleRepository)
    {
        _articleRepository =
            articleRepository;
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
                    .SearchAsync(query))
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