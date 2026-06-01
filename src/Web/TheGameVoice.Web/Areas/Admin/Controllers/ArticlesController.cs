using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Identity;
using TheGameVoice.Web.Areas.Admin.ViewModels.Articles;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;


namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class ArticlesController
    : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ISlugService _slugService;

    public ArticlesController(
        IUnitOfWork unitOfWork,
        ISlugService slugService)
    {
        _unitOfWork = unitOfWork;
        _slugService = slugService;
    }

    public async Task<IActionResult> Index()
    {
        var articles =
            await _unitOfWork.Articles.GetAllWithDetailsAsync();

        return View(articles);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CreateArticleViewModel();

        await PopulateArticleFormData(model);

        return View(model);
        
    }
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateArticleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateArticleFormData(model);

            return View(model);
        }

        var article = new Article
        {
            Title = model.Title,
            Summary = model.Summary,
            Content = model.Content,
            SeoTitle = model.SeoTitle,
            SeoDescription = model.SeoDescription,
            FeaturedImageId = model.FeaturedImageId,

            Status = ArticleStatus.Draft,
            CategoryId = model.CategoryId,
            Slug = await _slugService
                  .GenerateSlugAsync(model.Title)
        };

        await _unitOfWork.Articles
            .AddAsync(article);
        foreach (var tagId in model.SelectedTagIds)
        {
            article.ArticleTags.Add(
                new ArticleTag
                {
                    TagId = tagId
                });
        }
        foreach (var gameId in model.SelectedGameIds)
        {
            article.ArticleGames.Add(
                new ArticleGame
                {
                    GameId = gameId
                });
        }
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var article =
            await _unitOfWork.Articles.GetByIdAsync(id);


        if (article == null)
        {
            return NotFound();
        }
        var model = new EditArticleViewModel
        {
            Id = article.Id,

            Title = article.Title,

            Summary = article.Summary,

            Content = article.Content,

            SeoTitle = article.SeoTitle,

            SeoDescription = article.SeoDescription,

            FeaturedImageId = article.FeaturedImageId,

            CategoryId = article.CategoryId,

            SelectedTagIds = article.ArticleTags
                .Select(x => x.TagId)
                .ToList(),

            SelectedGameIds = article.ArticleGames
                .Select(x => x.GameId)
                .ToList()
        };
        await PopulateArticleFormData(model);

        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(
    EditArticleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateArticleFormData(model);

            return View(model);
        }

        var article =
            await _unitOfWork.Articles
                .GetByIdAsync(model.Id);

        if (article == null)
        {
            return NotFound();
        }
        article.ArticleTags.Clear();

        article.Title = model.Title;
        article.Summary = model.Summary;
        article.Content = model.Content;
        article.SeoTitle = model.SeoTitle;
        article.FeaturedImageId = model.FeaturedImageId;
        article.SeoDescription = model.SeoDescription;
        article.CategoryId = model.CategoryId;
        article.ArticleGames.Clear();

        foreach (var tagId in model.SelectedTagIds)
        {
            article.ArticleTags.Add(
                new ArticleTag
                {
                    ArticleId = article.Id,
                    TagId = tagId
                });

        }

        foreach (var gameId in model.SelectedGameIds)
        {
            article.ArticleGames.Add(
                new ArticleGame
                {
                    ArticleId = article.Id,
                    GameId = gameId
                });
        }
        _unitOfWork.Articles.Update(article);

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [Authorize(Roles =
    $"{Roles.Writer},{Roles.Editor},{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> SubmitForReview(Guid id)
    {
        var article =
            await _unitOfWork.Articles.GetByIdAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        article.Status = ArticleStatus.ReviewPending;

        _unitOfWork.Articles.Update(article);

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles =
    $"{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var article =
            await _unitOfWork.Articles.GetByIdAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        article.Status = ArticleStatus.Published;

        article.PublishedAt = DateTime.UtcNow;

        _unitOfWork.Articles.Update(article);

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    //Helper Metheod
    private async Task PopulateArticleFormData(
    ArticleFormViewModel model)
    {
        model.Categories =
            (await _unitOfWork.Categories.GetAllAsync())
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToList();

        model.Tags =
            (await _unitOfWork.Tags.GetAllAsync())
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToList();

        model.Games =
            (await _unitOfWork.Games.GetAllAsync())
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToList();

        model.MediaItems =
            (await _unitOfWork.Media.GetAllAsync())
            .Select(x => new MediaPickerItemViewModel
            {
                Id = x.Id,
                FileName = x.FileName,
                FilePath = x.FilePath
            })
            .ToList();
    }
}