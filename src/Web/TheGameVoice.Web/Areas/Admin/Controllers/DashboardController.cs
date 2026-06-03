using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Identity;
using TheGameVoice.Infrastructure.Identity.Entities;
using TheGameVoice.Web.Areas.Admin.ViewModels.Dashboard;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class DashboardController
    : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly UserManager<ApplicationUser>
        _userManager;

    public DashboardController(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var articles =
            await _unitOfWork.Articles
                .GetAllWithDetailsAsync();

        var media =
            await _unitOfWork.Media
                .GetAllAsync();

        var categories =
            await _unitOfWork.Categories
                .GetAllAsync();

        var tags =
            await _unitOfWork.Tags
                .GetAllAsync();

        var currentUser =
            await _userManager
                .GetUserAsync(User);

      
        var users =
            _userManager.Users.ToList();

        if (User.IsInRole(Roles.Author))
        {
            articles =
                articles
                    .Where(x =>
                        x.AuthorId ==
                        currentUser!.Id)
                    .ToList();
        }
        var model =
        new DashboardViewModel
        {
            DraftArticlesCount =
                articles.Count(x =>
                    x.Status ==
                    ArticleStatus.Draft),

            ReviewPendingCount =
                articles.Count(x =>
                    x.Status ==
                    ArticleStatus.ReviewPending),

            PublishedArticlesCount =
                articles.Count(x =>
                    x.Status ==
                    ArticleStatus.Published),

            ArchivedArticlesCount =
                articles.Count(x =>
                    x.Status ==
                    ArticleStatus.Archived),

            UsersCount =
                users.Count,

            MediaCount =
                media.Count,

            CategoriesCount =
                categories.Count,

            TagsCount =
                tags.Count,

            RecentArticles =
                articles
                    .OrderByDescending(x =>
                        x.CreatedAt)
                    .Take(10)
                    .Select(x =>
                        new DashboardArticleItemViewModel
                        {
                            Id = x.Id,
                            Title = x.Title,
                            Status = x.Status.ToString(),
                            CreatedAt = x.CreatedAt
                        })
                    .ToList()
        };

        return View(model);
    }
}