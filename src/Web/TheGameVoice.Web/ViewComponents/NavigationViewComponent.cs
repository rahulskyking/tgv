using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Common.Extensions;
using TheGameVoice.Web.Services;
using TheGameVoice.Web.ViewModels.Shared;

namespace TheGameVoice.Web.ViewComponents;

public class NavigationViewComponent
    : ViewComponent
{
    private readonly ICategoryRepository
        _categoryRepository;

    private readonly ISiteSegmentAccessor
        _siteSegment;

    public NavigationViewComponent(
        ICategoryRepository categoryRepository,
        ISiteSegmentAccessor siteSegment)
    {
        _categoryRepository =
            categoryRepository;

        _siteSegment = siteSegment;
    }

    public async Task<IViewComponentResult>
        InvokeAsync()
    {
        var segment = _siteSegment.Current;

        var categories =
            (await _categoryRepository
                .GetAllAsync())
            .Where(x => x.Segment.Includes(segment))
            .ToList();

        var request = HttpContext?.Request;

        var returnUrl =
            request == null
                ? "/"
                : $"{request.Path}{request.QueryString}";

        var model =
            new NavigationViewModel
            {
                Categories = categories,

                CurrentSegment = segment,

                ReturnUrl = string.IsNullOrWhiteSpace(returnUrl)
                    ? "/"
                    : returnUrl
            };

        return View(model);
    }
}
