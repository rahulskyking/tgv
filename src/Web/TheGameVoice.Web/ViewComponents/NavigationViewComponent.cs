using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Web.ViewModels.Shared;

namespace TheGameVoice.Web.ViewComponents;

public class NavigationViewComponent
    : ViewComponent
{
    private readonly ICategoryRepository
        _categoryRepository;

    public NavigationViewComponent(
        ICategoryRepository categoryRepository)
    {
        _categoryRepository =
            categoryRepository;
    }

    public async Task<IViewComponentResult>
        InvokeAsync()
    {
        var categories =
            await _categoryRepository
                .GetAllAsync();

        var model =
            new NavigationViewModel
            {
                Categories = categories
            };

        return View(model);
    }
}