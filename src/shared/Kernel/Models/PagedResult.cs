namespace Kernel.Models;

public class PagedResult<T>(IEnumerable<T> data, int page, int pageSize, int totalItems)
{
    public IEnumerable<T> Data { get; set; } = data;
    public int Page { get; set; } = page;
    public int PageSize { get; set; } = pageSize;
    public int TotalItems { get; set; } = totalItems;

    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
