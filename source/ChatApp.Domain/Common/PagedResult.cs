namespace ChatApp.Domain.Common;

public struct PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public uint TotalPages { get; set; }
    public uint ItemsFrom { get; set; }
    public uint ItemsTo { get; set; }
    public uint TotalItemsCount { get; set; }

    public PagedResult(IEnumerable<T> items, uint totalItemsCount, uint pageSize, uint pageNumber)
    {
        Items = items;
        TotalItemsCount = totalItemsCount;
        ItemsFrom = pageSize * (pageNumber - 1) + 1;
        ItemsTo = ItemsFrom + pageSize - 1;
        TotalPages = (uint)Math.Ceiling(totalItemsCount / (double)pageSize);
    }

    // Parameterless constructor for deserialization
#pragma warning disable CS8618
    public PagedResult() {}
#pragma warning restore CS8618
}
