namespace Application.Common.Pagination;

public sealed record PagedResponse<TItem>(
    IReadOnlyList<TItem> Items,
    int Page,
    int Limit,
    int Total);