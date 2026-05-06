using Application.Abstractions.Lots;
using Domain.Lots;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Lots;

internal sealed class LotRepository(ApplicationDbContext dbContext) : ILotRepository
{
    public async Task AddAsync(Lot lot, CancellationToken cancellationToken = default) =>
        await dbContext.Lots.AddAsync(lot, cancellationToken);

    public Task<Lot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Lots.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Lot>> GetListAsync(
        LotStatus? status,
        Guid? sellerId,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var query = BuildFilteredQuery(status, sellerId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip(Math.Max(0, skip))
            .Take(Math.Max(1, take));

        return await query.ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(LotStatus? status, Guid? sellerId, CancellationToken cancellationToken = default) =>
        BuildFilteredQuery(status, sellerId).CountAsync(cancellationToken);

    public void Remove(Lot lot) => dbContext.Lots.Remove(lot);

    private IQueryable<Lot> BuildFilteredQuery(LotStatus? status, Guid? sellerId)
    {
        var query = dbContext.Lots.AsQueryable();

        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (sellerId.HasValue) query = query.Where(x => x.SellerId == sellerId.Value);

        return query;
    }
}