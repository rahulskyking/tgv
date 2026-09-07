using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Domain.Common.Extensions;
using TheGameVoice.Web.Services;

namespace TheGameVoice.Web.Controllers;

/// <summary>
/// Flips the whole public site between PC / Console gaming and Mobile
/// gaming. The header toggle points here; the choice is stored in a cookie
/// and every public page then reads it through <see cref="ISiteSegmentAccessor"/>.
/// </summary>
public class SiteModeController : Controller
{
    private readonly ISiteSegmentAccessor _siteSegment;

    public SiteModeController(
        ISiteSegmentAccessor siteSegment)
    {
        _siteSegment = siteSegment;
    }

    [HttpGet]
    [Route("switch-mode/{mode}")]
    public IActionResult Switch(
        string mode,
        string? returnUrl = null)
    {
        _siteSegment.Set(
            GameSegmentExtensions.ParseSegment(mode));

        if (!string.IsNullOrWhiteSpace(returnUrl) &&
            Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(
            "Index",
            "Home");
    }
}
