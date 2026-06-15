using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Infrastructure.Identity.Entities;
using TheGameVoice.Web.ViewModels.Authors;

namespace TheGameVoice.Web.Controllers;

public class AuthorsController : Controller
{
    private readonly UserManager<ApplicationUser>
        _userManager;

    private readonly IArticleRepository
        _articleRepository;

    private readonly IMediaRepository
        _mediaRepository;

    public AuthorsController(
        UserManager<ApplicationUser> userManager,
        IArticleRepository articleRepository,
        IMediaRepository mediaRepository)
    {
        _userManager = userManager;

        _articleRepository = articleRepository;

        _mediaRepository = mediaRepository;
    }

    [Route("author/{slug}")]
    public async Task<IActionResult> Details(
        string slug)
    {
        var author =
            _userManager.Users
                .FirstOrDefault(x =>
                    x.Slug == slug);

        if (author == null)
        {
            return NotFound();
        }

        var articles =
            await _articleRepository
                .GetPublishedByAuthorAsync(
                    author.Id);

        string? avatarPath = null;

        if (author.AvatarImageId.HasValue)
        {
            var avatar =
                await _mediaRepository
                    .GetByIdAsync(
                        author.AvatarImageId.Value);

            avatarPath =
                avatar?.FilePath;
        }

        var model =
            new AuthorDetailsViewModel
            {
                FullName = author.FullName,

                Slug = author.Slug,

                Bio = author.Bio,

                TwitterUrl =
                    author.TwitterUrl,

                YouTubeUrl =
                    author.YouTubeUrl,

                WebsiteUrl =
                    author.WebsiteUrl,

                AvatarImagePath =
                    avatarPath,

                Articles = articles
            };

        return View(model);
    }
}