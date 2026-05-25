using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Web.Areas.Admin.ViewModels.Articles;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class ArticlesController
    : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;

    public ArticlesController(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var articles =
            await _unitOfWork.Articles.GetAllAsync();

        return View(articles);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateArticleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var article = new Article
        {
            Title = model.Title,
            Summary = model.Summary,
            Content = model.Content,
            SeoTitle = model.SeoTitle,
            SeoDescription = model.SeoDescription,

            Status = ArticleStatus.Draft,

            Slug = Guid.NewGuid().ToString()
        };

        await _unitOfWork.Articles
            .AddAsync(article);

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}