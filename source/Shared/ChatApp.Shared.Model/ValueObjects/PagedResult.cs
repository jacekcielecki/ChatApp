namespace ChatApp.Shared.Model.ValueObjects;

public struct PagedResult<T>
{
    public int TotalPages { get; set; }
    public int ItemsFrom { get; set; }
    public int ItemsTo { get; set; }
    public int TotalItemsCount { get; set; }
    public IEnumerable<T> Items { get; set; }

    public PagedResult(IEnumerable<T> items, int totalItemsCount, int pageSize, int pageNumber)
    {
        TotalItemsCount = totalItemsCount;
        TotalPages = (int)Math.Ceiling(totalItemsCount / (double)pageSize);

        // Prevent out-of-range page numbers
        if (pageNumber < 1 || pageNumber > TotalPages)
        {
            Items = Enumerable.Empty<T>();
            ItemsFrom = 0;
            ItemsTo = 0;
            return;
        }

        Items = items;

        ItemsFrom = (pageNumber - 1) * pageSize + 1;
        ItemsTo = Math.Min(ItemsFrom + pageSize - 1, totalItemsCount);
    }

    // Parameterless constructor for deserialization
#pragma warning disable CS8618
    public PagedResult() { }
#pragma warning restore CS8618
}
