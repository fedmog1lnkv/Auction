using Application.Abstractions.LotPhotos;
using Domain.Lots;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.LotPhotos;

internal sealed class LotPhotoRepository(ApplicationDbContext dbContext) : ILotPhotoRepository
{
    public Task<int> CountByLotIdAsync(Guid lotId, CancellationToken cancellationToken = default) =>
        dbContext.LotPhotos.CountAsync(x => x.LotId == lotId, cancellationToken);

    public async Task<IReadOnlyList<LotPhoto>> GetByLotIdAsync(
        Guid lotId,
        CancellationToken cancellationToken = default) =>
        await dbContext.LotPhotos
            .Where(x => x.LotId == lotId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

    public Task<LotPhoto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.LotPhotos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Dictionary<Guid, LotPhoto>> GetCoverPhotosByLotIdsAsync(
        IReadOnlyCollection<Guid> lotIds,
        CancellationToken cancellationToken = default)
    {
        if (lotIds.Count == 0)
            return new Dictionary<Guid, LotPhoto>();

        var covers = await dbContext.LotPhotos
            .Where(x => lotIds.Contains(x.LotId) && x.SortOrder == 1)
            .ToListAsync(cancellationToken);

        return covers.ToDictionary(x => x.LotId, x => x);
    }

    public async Task AddAsync(LotPhoto lotPhoto, CancellationToken cancellationToken = default) =>
        await dbContext.LotPhotos.AddAsync(lotPhoto, cancellationToken);

    public void Remove(LotPhoto lotPhoto) => dbContext.LotPhotos.Remove(lotPhoto);
}