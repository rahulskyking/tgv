using Microsoft.AspNetCore.Mvc.Rendering;
using TheGameVoice.Application.Common.Pagination;
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
  
}