using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Helpers;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Identity;
using TheGameVoice.Infrastructure.Identity.Entities;
using TheGameVoice.Web.Areas.Admin.ViewModels.Articles;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class ArticlesController : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISlugService _slugService;

    public ArticlesController(
        IUnitOfWork unitOfWork,
        ISlugService slugService,
        UserManager<ApplicationUser> userManager,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _slugService = slugService;
        _userManager = userManager;
        _cacheService = cacheService;
    }

    public async Task<IActionResult> Index()
    {
        var articles = await _unitOfWork.Articles.GetAllWithDetailsAsync();
        var currentUser = await _userManager.GetUserAsync(User);

        if (User.IsInRole(Roles.Author))
        {
            articles = articles.Where(x => x.AuthorId == currentUser!.Id).ToList();
        }

        return View(articles);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CreateArticleViewModel();
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser != null)
        {
            model.AuthorId = currentUser.Id;
        }
        await PopulateArticleFormData(model);
        model.StatusDisplay = "Draft";
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateArticleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateArticleFormData(model);
            return View(model);
        }

        var currentUser = await _userManager.GetUserAsync(User);

        Guid authorId = User.IsInRole(Roles.Author)
            ? currentUser!.Id
            : model.AuthorId;

        var article = new Article
        {
            Title = model.Title,
            Summary = model.Summary,
            Content = model.Content,
            SeoTitle = model.SeoTitle,
            SeoDescription = model.SeoDescription,
            FeaturedImageId = model.FeaturedImageId,
            AuthorId = authorId,
            Status = ArticleStatus.Draft,
            CategoryId = model.CategoryId,
            Slug = await _slugService.GenerateSlugAsync(model.Title),

            // Review fields only
            IsReview = model.IsReview,
            ReviewScore = model.IsReview ? model.ReviewScore : null,
            ReviewVerdict = model.IsReview ? model.ReviewVerdict : null,
            ReviewSummary = model.IsReview ? model.ReviewSummary : null
        };

        await _unitOfWork.Articles.AddAsync(article);

        // Tags
        foreach (var tagId in model.SelectedTagIds)
        {
            article.ArticleTags.Add(new ArticleTag
            {
                TagId = tagId
            });
        }

        // Games
        foreach (var gameId in model.SelectedGameIds)
        {
            article.ArticleGames.Add(new ArticleGame
            {
                GameId = gameId
            });
        }

        // Gallery
        var sortOrder = 1;

        foreach (var mediaId in model.SelectedGalleryImageIds)
        {
            article.ArticleMedia.Add(new ArticleMedia
            {
                MediaId = mediaId,
                SortOrder = sortOrder++
            });
        }

        // Videos
        var videoSortOrder = 1;

        foreach (var video in model.Videos)
        {
            if (string.IsNullOrWhiteSpace(video.VideoUrl))
                continue;

            article.ArticleVideos.Add(new ArticleVideo
            {
                Title = video.Title,
                VideoUrl = video.VideoUrl,
                SortOrder = videoSortOrder++
            });
        }

        // Save article first
        await _unitOfWork.SaveChangesAsync();

        // Save review points separately
        await SaveReviewPointsAsync(article, model);

        await _unitOfWork.SaveChangesAsync();

        _cacheService.RemoveMany(CacheKeys.HomePage);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var article = await _unitOfWork.Articles.GetByIdAsync(id);

        if (article == null) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);

        if (User.IsInRole(Roles.Author) && article.AuthorId != currentUser!.Id)
        {
            return Forbid();
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
            AuthorId = article.AuthorId,
            SelectedTagIds = article.ArticleTags.Select(x => x.TagId).ToList(),
            SelectedGameIds = article.ArticleGames.Select(x => x.GameId).ToList(),
            SelectedGalleryImageIds = article.ArticleMedia.OrderBy(x => x.SortOrder).Select(x => x.MediaId).ToList()
        };

        model.GalleryImages = article.ArticleMedia
            .OrderBy(x => x.SortOrder)
            .Select(x => new MediaPickerItemViewModel
            {
                Id = x.Media.Id,
                FileName = x.Media.FileName,
                FilePath = x.Media.FilePath
            }).ToList();

        model.Videos = article.ArticleVideos
            .OrderBy(x => x.SortOrder)
            .Select(x => new ArticleVideoInputViewModel
            {
                Title = x.Title,
                VideoUrl = x.VideoUrl
            }).ToList();

        model.IsReview = article.IsReview;
        model.ReviewScore = article.ReviewScore;
        model.ReviewVerdict = article.ReviewVerdict;
        model.ReviewSummary = article.ReviewSummary;

        model.GoodReviewPoints = article.ReviewPoints
            .Where(x => x.Type == ReviewPointType.Good)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => x.Text)
            .ToList();

        model.BadReviewPoints = article.ReviewPoints
            .Where(x => x.Type == ReviewPointType.Bad)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => x.Text)
            .ToList();

        await PopulateArticleFormData(model);
        model.StatusDisplay = article.Status.ToString();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditArticleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateArticleFormData(model);
            return View(model);
        }

        var article = await _unitOfWork.Articles.GetByIdAsync(model.Id);

        if (article == null)
            return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);

        if (User.IsInRole(Roles.Author) &&
            article.AuthorId != currentUser!.Id)
        {
            return Forbid();
        }

        // Basic fields

        article.Title = model.Title;
        article.Summary = model.Summary;
        article.Content = model.Content;
        article.SeoTitle = model.SeoTitle;
        article.SeoDescription = model.SeoDescription;
        article.FeaturedImageId = model.FeaturedImageId;
        article.CategoryId = model.CategoryId;

        if (User.IsInRole(Roles.Author))
            article.AuthorId = currentUser!.Id;
        else
            article.AuthorId = model.AuthorId;

        // Review fields only

        article.IsReview = model.IsReview;
        article.ReviewScore = model.IsReview
            ? model.ReviewScore
            : null;

        article.ReviewVerdict =
          model.ReviewScore.HasValue
              ? ReviewHelper.GetVerdict(model.ReviewScore.Value)
              : null;

        article.ReviewSummary = model.IsReview
            ? model.ReviewSummary
            : null;

        // Relations

        article.ArticleTags.Clear();

        foreach (var tagId in model.SelectedTagIds)
        {
            article.ArticleTags.Add(new ArticleTag
            {
                ArticleId = article.Id,
                TagId = tagId
            });
        }

        article.ArticleGames.Clear();

        foreach (var gameId in model.SelectedGameIds)
        {
            article.ArticleGames.Add(new ArticleGame
            {
                ArticleId = article.Id,
                GameId = gameId
            });
        }

        article.ArticleMedia.Clear();

        var mediaOrder = 1;

        foreach (var mediaId in model.SelectedGalleryImageIds)
        {
            article.ArticleMedia.Add(new ArticleMedia
            {
                ArticleId = article.Id,
                MediaId = mediaId,
                SortOrder = mediaOrder++
            });
        }

        article.ArticleVideos.Clear();

        var videoOrder = 1;

        foreach (var video in model.Videos)
        {
            if (string.IsNullOrWhiteSpace(video.VideoUrl))
                continue;

            article.ArticleVideos.Add(new ArticleVideo
            {
                ArticleId = article.Id,
                Title = video.Title,
                VideoUrl = video.VideoUrl,
                SortOrder = videoOrder++
            });
        }

        // Save article first

        await _unitOfWork.SaveChangesAsync();

        // Save review points separately

        await SaveReviewPointsAsync(article, model);

        await _unitOfWork.SaveChangesAsync();

        _cacheService.RemoveMany(CacheKeys.HomePage);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Author},{Roles.Editor},{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> SubmitForReview(Guid id)
    {
        var article = await _unitOfWork.Articles.GetByIdAsync(id);
        if (article == null) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (User.IsInRole(Roles.Author) && article.AuthorId != currentUser!.Id)
        {
            return Forbid();
        }

        article.Status = ArticleStatus.ReviewPending;
        _unitOfWork.Articles.Update(article);

        await _unitOfWork.SaveChangesAsync();
        _cacheService.RemoveMany(CacheKeys.HomePage);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Editor},{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var article = await _unitOfWork.Articles.GetByIdAsync(id);
        if (article == null) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);

        article.Status = ArticleStatus.Published;
        article.PublishedAt = DateTime.UtcNow;
        article.PublishedById = currentUser?.Id;

        _unitOfWork.Articles.Update(article);

        await _unitOfWork.SaveChangesAsync();
        _cacheService.RemoveMany(CacheKeys.HomePage);

        return RedirectToAction(nameof(Index));
    }

    //Helper Method
    private async Task PopulateArticleFormData(ArticleFormViewModel model)
    {
        model.Categories = (await _unitOfWork.Categories.GetAllAsync())
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToList();

        model.Tags = (await _unitOfWork.Tags.GetAllAsync())
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToList();

        model.Games = (await _unitOfWork.Games.GetAllAsync())
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToList();

        model.MediaItems = (await _unitOfWork.Media.GetAllAsync())
            .Select(x => new MediaPickerItemViewModel { Id = x.Id, FileName = x.FileName, FilePath = x.FilePath })
            .ToList();

        var users = _userManager.Users.ToList();
        model.Authors = users
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.FullName })
            .ToList();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{Roles.Editor},{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> Reject(Guid id)
    {
        var article = await _unitOfWork.Articles.GetByIdAsync(id);
        if (article == null) return NotFound();

        article.Status = ArticleStatus.Rejected;
        _unitOfWork.Articles.Update(article);

        await _unitOfWork.SaveChangesAsync();
        _cacheService.RemoveMany(CacheKeys.HomePage);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> Archive(Guid id)
    {
        var article = await _unitOfWork.Articles.GetByIdAsync(id);
        if (article == null) return NotFound();

        article.Status = ArticleStatus.Archived;
        _unitOfWork.Articles.Update(article);

        await _unitOfWork.SaveChangesAsync();
        _cacheService.RemoveMany(CacheKeys.HomePage);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{Roles.Editor},{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> Restore(Guid id)
    {
        var article = await _unitOfWork.Articles.GetByIdAsync(id);
        if (article == null) return NotFound();

        article.Status = ArticleStatus.Draft;
        _unitOfWork.Articles.Update(article);

        await _unitOfWork.SaveChangesAsync();
        _cacheService.RemoveMany(CacheKeys.HomePage);

        return RedirectToAction(nameof(Index));
    }

    #region Preview
    [HttpGet]
    public async Task<IActionResult> Preview(Guid id)
    {
        var article = await _unitOfWork.Articles.GetByIdAsync(id);
        if (article == null) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (User.IsInRole(Roles.Author) && article.AuthorId != currentUser!.Id)
        {
            return Forbid();
        }

        return View(article);
    }
    #endregion
    private async Task SaveReviewPointsAsync(
    Article article,
    ArticleFormViewModel model)
    {
        // Delete existing review points
        await _unitOfWork.Articles.DeleteReviewPointsAsync(article.Id);

        if (!model.IsReview)
            return;

        var reviewPoints = new List<ArticleReviewPoint>();

        var order = 1;

        foreach (var point in model.GoodReviewPoints
                     .Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            reviewPoints.Add(new ArticleReviewPoint
            {
                ArticleId = article.Id,
                Type = ReviewPointType.Good,
                Text = point.Trim(),
                DisplayOrder = order++
            });
        }

        order = 1;

        foreach (var point in model.BadReviewPoints
                     .Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            reviewPoints.Add(new ArticleReviewPoint
            {
                ArticleId = article.Id,
                Type = ReviewPointType.Bad,
                Text = point.Trim(),
                DisplayOrder = order++
            });
        }

        await _unitOfWork.Articles.AddReviewPointsAsync(reviewPoints);
    }

}