using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Infrastructure.Identity;
using TheGameVoice.Infrastructure.Identity.Entities;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;
using TheGameVoice.Web.Areas.Admin.ViewModels.Users;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

[Authorize(
    Roles =
    $"{Roles.Admin}," +
    $"{Roles.SuperAdmin}")]
public class UsersController : BaseAdminController
{
    private readonly UserManager<ApplicationUser>
        _userManager;

    private readonly RoleManager<IdentityRole<Guid>>
        _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISlugService _slugService;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IUnitOfWork unitOfWork,
        ISlugService slugService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _slugService = slugService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users =
            _userManager.Users.ToList();

        var model =
            new List<UserListItemViewModel>();

        foreach (var user in users)
        {
            var roles =
                await _userManager
                    .GetRolesAsync(user);

            model.Add(
                new UserListItemViewModel
                {
                    Id = user.Id,

                    FullName = user.FullName,

                    UserName =
                        user.UserName ?? "",

                    Email =
                        user.Email ?? "",

                    IsActive =
                        user.IsActive,

                    Role =
                        roles.FirstOrDefault()
                        ?? "-"
                });
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model =
    new CreateUserViewModel();

        model.Roles =
        [
            new SelectListItem(
        Roles.Author,
        Roles.Author),

    new SelectListItem(
        Roles.Editor,
        Roles.Editor),

    new SelectListItem(
        Roles.Admin,
        Roles.Admin)
        ];

        return View(model);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Roles =
            [
                new SelectListItem(
            Roles.Author,
            Roles.Author),

        new SelectListItem(
            Roles.Editor,
            Roles.Editor),

        new SelectListItem(
            Roles.Admin,
            Roles.Admin)
            ];

            return View(model);
        }
        var user =
            new ApplicationUser
            {
                FullName =
                    model.FullName,

                UserName =
                    model.UserName,

                Email =
                    model.Email,

                IsActive = true
            };

        var result =
            await _userManager
                .CreateAsync(
                    user,
                    model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    "",
                    error.Description);
            }

            return View(model);
        }
        if (!await _roleManager
    .RoleExistsAsync(model.Role))
        {
            ModelState.AddModelError(
                "",
                "Invalid role.");

            return View(model);
        }
        await _userManager.AddToRoleAsync(
            user,
            model.Role);

        return RedirectToAction(
            nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var user =
            await _userManager.FindByIdAsync(
                id.ToString());

        if (user == null)
        {
            return NotFound();
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        var mediaItems =
                 (await _unitOfWork.Media.GetAllAsync())
                 .Select(x => new MediaPickerItemViewModel
                 {
                     Id = x.Id,
                     FileName = x.FileName,
                     FilePath = x.FilePath
                 })
                 .ToList();

        var model =
            new EditUserViewModel
            {
                Id = user.Id,

                FullName = user.FullName,

                UserName = user.UserName ?? "",

                

                Email = user.Email ?? "",

                IsActive = user.IsActive,

                Slug = user.Slug,

                Bio = user.Bio,

                AvatarImageId = user.AvatarImageId,

                MediaItems = mediaItems,

                TwitterUrl = user.TwitterUrl,

                YouTubeUrl = user.YouTubeUrl,

                WebsiteUrl = user.WebsiteUrl,

                Role = roles.FirstOrDefault() ?? ""
            };

        model.Roles =
             [
                 new SelectListItem(
            Roles.Author,
            Roles.Author),

        new SelectListItem(
            Roles.Editor,
            Roles.Editor),

        new SelectListItem(
            Roles.Admin,
            Roles.Admin)
             ];


  

        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    EditUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Roles =
             [
                 new SelectListItem(
            Roles.Author,
            Roles.Author),

        new SelectListItem(
            Roles.Editor,
            Roles.Editor),

        new SelectListItem(
            Roles.Admin,
            Roles.Admin)
             ];

            return View(model);
        }

        var user =
            await _userManager.FindByIdAsync(
                model.Id.ToString());

        if (user == null)
        {
            return NotFound();
        }

        user.FullName =
            model.FullName;

        if (string.IsNullOrWhiteSpace(model.Slug))
        {
            await _slugService
          .GenerateAuthorSlugAsync(
              model.FullName);
        }
      

        user.Bio = model.Bio;

        user.AvatarImageId = model.AvatarImageId;

        user.TwitterUrl = model.TwitterUrl;

        user.YouTubeUrl = model.YouTubeUrl;

        user.WebsiteUrl = model.WebsiteUrl;
        user.UserName =
            model.UserName;

        user.Email =
            model.Email;

        user.IsActive =
            model.IsActive;

        var currentRoles =
            await _userManager
                .GetRolesAsync(user);

        await _userManager
            .RemoveFromRolesAsync(
                user,
                currentRoles);

        await _userManager
            .AddToRoleAsync(
                user,
                model.Role);

        await _userManager
            .UpdateAsync(user);

        return RedirectToAction(
            nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var user =
            await _userManager.FindByIdAsync(
                id.ToString());

        if (user == null)
        {
            return NotFound();
        }

        user.IsActive =
            !user.IsActive;

        await _userManager
            .UpdateAsync(user);

        return RedirectToAction(
            nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> ResetPassword(
    Guid id)
    {
        var user =
            await _userManager.FindByIdAsync(
                id.ToString());

        if (user == null)
        {
            return NotFound();
        }

        var model =
            new ResetPasswordViewModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? ""
            };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
    ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user =
            await _userManager.FindByIdAsync(
                model.UserId.ToString());

        if (user == null)
        {
            return NotFound();
        }

        var token =
            await _userManager
                .GeneratePasswordResetTokenAsync(
                    user);

        var result =
            await _userManager
                .ResetPasswordAsync(
                    user,
                    token,
                    model.NewPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    "",
                    error.Description);
            }

            return View(model);
        }

        TempData["Success"] =
            "Password reset successfully.";

        return RedirectToAction(
            nameof(Index));
    }


}