namespace TheGameVoice.Application.Common.Pagination;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; }
        = Array.Empty<T>();

    public int TotalCount { get; set; }

    public int CurrentPage { get; set; }

    public int PageSize { get; set; }

    public int TotalPages =>
        PageSize == 0
            ? 0
            : (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasMultiplePages =>
    TotalPages > 1;
    public bool HasPreviousPage =>
        CurrentPage > 1;

    public bool HasNextPage =>

        CurrentPage < TotalPages;
    public int StartItem =>
    TotalCount == 0
        ? 0
        : ((CurrentPage - 1) * PageSize) + 1;

    public int EndItem =>
        Math.Min(CurrentPage * PageSize, TotalCount);
}