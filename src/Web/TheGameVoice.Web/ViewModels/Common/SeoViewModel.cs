namespace TheGameVoice.Web.ViewModels.Common;

public class SeoViewModel
{
    public string Title { get; set; }
        = string.Empty;

    public string Description { get; set; }
        = string.Empty;

    public string? ImageUrl { get; set; }

    public string? CanonicalUrl { get; set; }
}