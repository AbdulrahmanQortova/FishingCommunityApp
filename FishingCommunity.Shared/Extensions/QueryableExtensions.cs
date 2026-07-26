using FishingCommunity.Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginatedList<TDestination>> ToPaginatedListAsync<TDestination>(
        this IQueryable<TDestination> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await source.CountAsync(cancellationToken);

        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<TDestination>(items, totalCount, pageNumber, pageSize);
    }
}