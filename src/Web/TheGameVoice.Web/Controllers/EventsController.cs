using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Web.ViewModels.Events;

namespace TheGameVoice.Web.Controllers;

public class EventsController : Controller
{
    private readonly IEventRepository _events;
    public EventsController(IEventRepository events) => _events = events;

    [HttpGet("events")]
    public async Task<IActionResult> Index(bool archived = false)
    {
        ViewData["Archived"] = archived;
        return View((await _events.GetAllAsync()).Where(x => x.IsActive != archived).ToList());
    }

    [HttpGet("events/{slug}")]
    public async Task<IActionResult> Details(string slug, int page = 1)
    {
        if (page < 1) return NotFound();
        var item = await _events.GetBySlugAsync(slug);
        if (item is null) return NotFound();
        const int pageSize = 12;
        var count = await _events.CountPublishedArticlesAsync(item.Id);
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);
        if (page > Math.Max(1, totalPages)) return NotFound();
        ViewData["CanonicalUrl"] = Url.Action(nameof(Details), "Events",
            new { slug = item.Slug, page = page > 1 ? (int?)page : null }, Request.Scheme);
        return View(new EventDetailsViewModel
        {
            Event = item, Page = page, TotalPages = totalPages, TotalArticles = count,
            Articles = await _events.GetPublishedArticlesAsync(item.Id, page, pageSize)
        });
    }
}
