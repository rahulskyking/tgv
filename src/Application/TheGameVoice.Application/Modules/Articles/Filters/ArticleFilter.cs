using System.ComponentModel.DataAnnotations;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Application.Modules.Articles.Filters;

public class ArticleFilter
{
    public string? Search { get; set; }

    public ArticleStatus? Status { get; set; }

    /// <summary>
    /// Audience segment filter for the admin list: null = show everything,
    /// PcConsole / Mobile = only articles targeting that segment (articles
    /// marked for both appear under either value).
    /// </summary>
    public GameSegment? Segment { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? AuthorId { get; set; }

    public ArticleSort SortBy { get; set; }
        = ArticleSort.Latest;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(10, 100)]
    public int PageSize { get; set; } = 10;
}