using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Web.ViewModels.Events;

public class EventDetailsViewModel
{
    public GamingEvent Event { get; set; } = default!;
    public IReadOnlyList<Article> Articles { get; set; } = Array.Empty<Article>();
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalArticles { get; set; }
}
