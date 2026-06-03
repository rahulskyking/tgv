using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Identity;
using TheGameVoice.Infrastructure.Identity.Entities;
using TheGameVoice.Web.Areas.Admin.ViewModels.Articles;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;


namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class ArticlesController
    : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly UserManager<ApplicationUser>
    _userManager;
    private readonly ISlugService _slugService;

    public ArticlesController(
        IUnitOfWork unitOfWork,
        ISlugService slugService,
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _slugService = slugService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var articles =
            await _unitOfWork.Articles
                .GetAllWithDetailsAsync();

        var currentUser =
            await _userManager.GetUserAsync(User);

        if (User.IsInRole(Roles.Author))
        {
            articles =
                articles
                    .Where(x =>
                        x.AuthorId ==
                        currentUser!.Id)
                    .ToList();
        }

        return View(articles);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CreateArticleViewModel();
        var currentUser =
    await _userManager.GetUserAsync(User);

        if (currentUser != null)
        {
            model.AuthorId =
                currentUser.Id;
        }
        await PopulateArticleFormData(model);
        model.StatusDisplay = "Draft";
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
        var currentUser =
    await _userManager.GetUserAsync(User);

        Guid authorId;

        if (User.IsInRole(Roles.Author))
        {
            authorId = currentUser!.Id;
        }
        else
        {
            authorId = model.AuthorId;
        }
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
        var sortOrder = 1;

        foreach (var mediaId in model.SelectedGalleryImageIds)
        {
            article.ArticleMedia.Add(
                new ArticleMedia
                {
                    MediaId = mediaId,
                    SortOrder = sortOrder++
                });
        }

        var videoSortOrder = 1;

        foreach (var video in model.Videos)
        {
            if (string.IsNullOrWhiteSpace(
                video.VideoUrl))
            {
                continue;
            }

            article.ArticleVideos.Add(
                new ArticleVideo
                {
                    Title = video.Title,

                    VideoUrl = video.VideoUrl,

                    SortOrder = videoSortOrder++
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
        var currentUser =
    await _userManager.GetUserAsync(User);

        if (User.IsInRole(Roles.Author) &&
            article.AuthorId != currentUser!.Id)
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

            SelectedTagIds = article.ArticleTags
                .Select(x => x.TagId)
                .ToList(),

            SelectedGameIds = article.ArticleGames
                .Select(x => x.GameId)
                .ToList(),

            SelectedGalleryImageIds =
            article.ArticleMedia
                .OrderBy(x => x.SortOrder)
                .Select(x => x.MediaId)
                .ToList()

        };
        model.GalleryImages =
    article.ArticleMedia
        .OrderBy(x => x.SortOrder)
        .Select(x => new MediaPickerItemViewModel
        {
            Id = x.Media.Id,
            FileName = x.Media.FileName,
            FilePath = x.Media.FilePath
        })
        .ToList();

        model.Videos =
    article.ArticleVideos
        .OrderBy(x => x.SortOrder)
        .Select(x =>
            new ArticleVideoInputViewModel
            {
                Title = x.Title,

                VideoUrl = x.VideoUrl
            })
        .ToList();
        await PopulateArticleFormData(model);
        model.StatusDisplay = article.Status.ToString();
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
        var currentUser =
    await _userManager.GetUserAsync(User);

        if (User.IsInRole(Roles.Author) &&
            article.AuthorId != currentUser!.Id)
        {
            return Forbid();
        }

        article.ArticleTags.Clear();
        article.ArticleVideos.Clear();
        article.ArticleMedia.Clear();
        article.ArticleGames.Clear();


        if (User.IsInRole(Roles.Author))
        {
            article.AuthorId =
                currentUser!.Id;
        }
        else
        {
            article.AuthorId =
                model.AuthorId;
        }


        article.Title = model.Title;
        article.Summary = model.Summary;
        article.Content = model.Content;
        article.SeoTitle = model.SeoTitle;
        article.FeaturedImageId = model.FeaturedImageId;
        article.SeoDescription = model.SeoDescription;
        article.CategoryId = model.CategoryId;


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
        var sortOrder = 1;

        foreach (var mediaId in model.SelectedGalleryImageIds)
        {
            article.ArticleMedia.Add(
                new ArticleMedia
                {
                    ArticleId = article.Id,
                    MediaId = mediaId,
                    SortOrder = sortOrder++
                });
        }
        _unitOfWork.Articles.Update(article);

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [Authorize(Roles =
    $"{Roles.Author},{Roles.Editor},{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> SubmitForReview(Guid id)
    {
        var article =
            await _unitOfWork.Articles.GetByIdAsync(id);

        if (article == null)
        {
            return NotFound();
        }
        var currentUser =
    await _userManager.GetUserAsync(User);

        if (User.IsInRole(Roles.Author) &&
            article.AuthorId != currentUser!.Id)
        {
            return Forbid();
        }
        article.Status = ArticleStatus.ReviewPending;

        _unitOfWork.Articles.Update(article);

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(
    Roles =
    $"{Roles.Editor}," +
    $"{Roles.Admin}," +
    $"{Roles.SuperAdmin}")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var article =
            await _unitOfWork.Articles.GetByIdAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        var currentUser =
      await _userManager.GetUserAsync(User);

        article.Status =
            ArticleStatus.Published;

        article.PublishedAt =
            DateTime.UtcNow;

        article.PublishedById =
            currentUser?.Id;

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
        var users = _userManager.Users.ToList();

        model.Authors =
            users
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                })
                .ToList();

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(
Roles =
$"{Roles.Editor}," +
$"{Roles.Admin}," +
$"{Roles.SuperAdmin}")]
    public async Task<IActionResult> Reject(Guid id)
    {
        var article =
            await _unitOfWork.Articles.GetByIdAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        article.Status =
            ArticleStatus.Rejected;

        _unitOfWork.Articles.Update(article);

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(
    Roles =
    $"{Roles.Admin}," +
    $"{Roles.SuperAdmin}")]
    public async Task<IActionResult> Archive(Guid id)
    {
        var article =
            await _unitOfWork.Articles.GetByIdAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        article.Status =
            ArticleStatus.Archived;

        _unitOfWork.Articles.Update(article);

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(
Roles =
$"{Roles.Editor}," +
$"{Roles.Admin}," +
$"{Roles.SuperAdmin}")]
    public async Task<IActionResult> Restore(Guid id)
    {
        var article =
            await _unitOfWork.Articles.GetByIdAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        article.Status =
            ArticleStatus.Draft;

        _unitOfWork.Articles.Update(article);

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    #region Preview
    [HttpGet]
    public async Task<IActionResult> Preview(Guid id)
    {
        var article =
            await _unitOfWork.Articles
                .GetByIdAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        var currentUser =
            await _userManager.GetUserAsync(User);

        if (User.IsInRole(Roles.Author) &&
            article.AuthorId != currentUser!.Id)
        {
            return Forbid();
        }

        return View(article);
    }
    #endregion
}