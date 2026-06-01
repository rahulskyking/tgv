using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Web.ViewModels.Shared;

public class NavigationViewModel
{
    public IReadOnlyList<Category> Categories
    { get; set; }
        = new List<Category>();
}