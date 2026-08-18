using Microsoft.AspNetCore.Mvc.Rendering;
using TheGameVoice.Application.Common.Pagination;
using TheGameVoice.Application.Modules.Articles;
using TheGameVoice.Application.Modules.Articles.Filters;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Web.ViewModels.Shared;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Articles;

public class ArticleIndexViewModel
{
    public PagedResult<Article> Articles { get; set; }
        = new();

    public ArticleFilter Filter { get; set; }
        = new();

    public Dictionary<Guid, string> AuthorNames { get; set; }
        = new();

    public List<SelectListItem> Categories { get; set; }
        = new();

    public List<SelectListItem> Authors { get; set; }
        = new();

    public List<SelectListItem> Statuses { get; set; }
        = new();
    public List<SelectListItem> SortOptions { get; set; }
    = new();

    /// <summary>Article counts and view totals for the current filter.</summary>
    public ArticleStatsSummary Summary { get; set; }
        = new();

    /// <summary>True when the signed-in user may open other authors' stats.</summary>
    public bool CanViewAuthorStats { get; set; }
  
}