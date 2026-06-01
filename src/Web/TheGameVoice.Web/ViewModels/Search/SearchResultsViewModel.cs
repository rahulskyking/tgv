using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Web.ViewModels.Search;

public class SearchResultsViewModel
{
    public string Query { get; set; }
        = string.Empty;

    public IReadOnlyList<Article>
        Articles
    { get; set; }
        = new List<Article>();
}