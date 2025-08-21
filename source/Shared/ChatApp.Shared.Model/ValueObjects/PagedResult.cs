namespace ChatApp.Shared.Model.ValueObjects;

public struct PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalPages { get; set; }
    public int ItemsFrom { get; set; }
    public int ItemsTo { get; set; }
    public int TotalItemsCount { get; set; }

    public PagedResult(IEnumerable<T> items, int totalItemsCount, int pageSize, int pageNumber)
    {
        Items = items;
        TotalItemsCount = totalItemsCount;
        ItemsFrom = pageSize * (pageNumber - 1) + 1;
        ItemsTo = ItemsFrom + pageSize - 1;
        TotalPages = (int)Math.Ceiling(totalItemsCount / (double)pageSize);
    }

    // Parameterless constructor for deserialization
#pragma warning disable CS8618
    public PagedResult() {}
#pragma warning restore CS8618
}
