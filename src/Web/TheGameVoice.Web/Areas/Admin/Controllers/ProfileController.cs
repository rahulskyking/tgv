using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Application.Modules.Articles.Filters;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Identity.Entities;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;
using TheGameVoice.Web.Areas.Admin.ViewModels.Profile;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

/// <summary>
/// Every signed-in writer manages their own public profile here — bio,
/// avatar, links and password. UsersController stays admin-only; this one is
/// open to anyone who can reach the admin, but always scoped to *themselves*.
/// </summary>
public class ProfileController : BaseAdminController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISlugService _slugService;

    public ProfileController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUnitOfWork unitOfWork,
        ISlugService slugService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _unitOfWork = unitOfWork;
        _slugService = slugService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound();
        }

        return View(await BuildAsync(user));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(MyProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            var invalid = await BuildAsync(user);

            invalid.FullName = model.FullName;
            invalid.Bio = model.Bio;
            invalid.Slug = model.Slug;
            invalid.TwitterUrl = model.TwitterUrl;
            invalid.YouTubeUrl = model.YouTubeUrl;
            invalid.WebsiteUrl = model.WebsiteUrl;
            invalid.SteamUrl = model.SteamUrl;
            invalid.AvatarImageId = model.AvatarImageId;

            return View(invalid);
        }

        user.FullName = model.FullName;
        user.Bio = model.Bio;
        user.AvatarImageId = model.AvatarImageId;
        user.TwitterUrl = model.TwitterUrl;
        user.YouTubeUrl = model.YouTubeUrl;
        user.WebsiteUrl = model.WebsiteUrl;
        user.SteamUrl = model.SteamUrl;

        // A writer without a slug has no public page, so always make sure
        // there is one.
        user.Slug =
            string.IsNullOrWhiteSpace(model.Slug)
                ? await _slugService.GenerateAuthorSlugAsync(model.FullName)
                : model.Slug.Trim().ToLowerInvariant();

        if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailResult = await _userManager.SetEmailAsync(user, model.Email);

            if (!emailResult.Succeeded)
            {
                foreach (var error in emailResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(await BuildAsync(user));
            }
        }

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(await BuildAsync(user));
        }

        TempData["Success"] = "Your profile has been updated.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please check the password fields and try again.";

            return RedirectToAction(nameof(Index));
        }

        var result =
            await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

        if (!result.Succeeded)
        {
            TempData["Error"] =
                string.Join(" ", result.Errors.Select(error => error.Description));

            return RedirectToAction(nameof(Index));
        }

        await _signInManager.RefreshSignInAsync(user);

        TempData["Success"] = "Your password has been changed.";

        return RedirectToAction(nameof(Index));
    }

    private async Task<MyProfileViewModel> BuildAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var media = await _unitOfWork.Media.GetAllAsync();

        var summary =
            await _unitOfWork.Articles.GetSummaryAsync(
                new ArticleFilter { AuthorId = user.Id });

        var lastPublished =
            await _unitOfWork.Articles.GetPagedAsync(
                new ArticleFilter
                {
                    AuthorId = user.Id,
                    Status = ArticleStatus.Published,
                    SortBy = ArticleSort.Latest,
                    Page = 1,
                    PageSize = 10
                });

        var avatarPath =
            media.FirstOrDefault(x => x.Id == user.AvatarImageId)?.FilePath;

        return new MyProfileViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Slug = user.Slug,
            Bio = user.Bio,
            AvatarImageId = user.AvatarImageId,
            AvatarImagePath = avatarPath,
            TwitterUrl = user.TwitterUrl,
            YouTubeUrl = user.YouTubeUrl,
            WebsiteUrl = user.WebsiteUrl,
            SteamUrl = user.SteamUrl,
            Role = roles.FirstOrDefault() ?? "—",

            MediaItems = media
                .Select(x => new MediaPickerItemViewModel
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    FilePath = x.FilePath
                })
                .ToList(),

            PublishedArticles = summary.PublishedArticles,
            DraftArticles = summary.DraftArticles,
            PendingArticles = summary.ReviewPendingArticles,
            TotalViews = summary.TotalViews,

            LastPublishedAtUtc = lastPublished.Items
                .Select(x => x.PublishedAt)
                .FirstOrDefault()
        };
    }
}
