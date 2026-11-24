using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Constants;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Domain.Common.Extensions;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Infrastructure.Identity;
using TheGameVoice.Web.Areas.Admin.ViewModels.HomepagePins;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

[Authorize(Roles = $"{Roles.Editor},{Roles.Admin},{Roles.SuperAdmin}")]
[AutoValidateAntiforgeryToken]
public class HomepagePinsController : BaseAdminController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public HomepagePinsController(IUnitOfWork unitOfWork, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var articles = await _unitOfWork.Articles.GetHomepagePinnedAsync();
        var pcPosition = 0;
        var mobilePosition = 0;
        var rows = new List<HomepagePinRowViewModel>();
        foreach (var article in articles)
        {
            rows.Add(new HomepagePinRowViewModel
            {
                Article = article,
                PcPlacement = DescribePlacement(article, GameSegment.PcConsole, ref pcPosition),
                MobilePlacement = DescribePlacement(article, GameSegment.Mobile, ref mobilePosition)
            });
        }

        return View(rows);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrder(UpdateHomepagePinViewModel model)
    {
        if (!ModelState.IsValid || model.Id == Guid.Empty)
        {
            TempData["PinError"] = "Order must be a whole number between 1 and 9999. Refresh and try again.";
            return RedirectToAction(nameof(Index));
        }

        var article = await _unitOfWork.Articles.GetByIdAsync(model.Id);
        if (article is null) return NotFound();
        if (!article.IsHomepagePinned || article.HomepagePinOrder != model.ExpectedOrder)
        {
            TempData["PinError"] = "This pin has changed since you opened the page. Review the current list and try again.";
            return RedirectToAction(nameof(Index));
        }

        article.HomepagePinOrder = model.HomepagePinOrder!.Value;
        await SavePins();
        TempData["PinSuccess"] = "Homepage order saved. The list now reflects the new priority.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Unpin(Guid id, int? expectedOrder)
    {
        if (!ModelState.IsValid || id == Guid.Empty || expectedOrder is null)
            return BadRequest();

        var article = await _unitOfWork.Articles.GetByIdAsync(id);
        if (article is null) return NotFound();
        if (!article.IsHomepagePinned || article.HomepagePinOrder != expectedOrder)
        {
            TempData["PinError"] = "This pin has changed since you opened the page. Review the current list before unpinning.";
            return RedirectToAction(nameof(Index));
        }

        article.IsHomepagePinned = false;
        await SavePins();
        TempData["PinSuccess"] = "Article unpinned. It can still appear naturally among the latest stories.";
        return RedirectToAction(nameof(Index));
    }

    private async Task SavePins()
    {
        await _unitOfWork.SaveChangesAsync();
        _cache.RemoveMany(CacheKeys.AllHomePageKeys());
    }

    private static string DescribePlacement(Article article, GameSegment segment, ref int position)
    {
        if (!article.Segment.Includes(segment)) return "Not assigned";
        if (article.Status != ArticleStatus.Published) return "Not live — unpublished";

        position++;
        return position switch
        {
            1 => "Main hero",
            <= 9 => $"Grid position {position}",
            _ => $"Outside top 9 (position {position})"
        };
    }
}
