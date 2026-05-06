using Domain.Lots;

namespace Application.Abstractions.LotPhotos;

public interface ILotPhotoRepository
{
    Task<int> CountByLotIdAsync(Guid lotId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LotPhoto>> GetByLotIdAsync(Guid lotId, CancellationToken cancellationToken = default);
    Task<LotPhoto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, LotPhoto>> GetCoverPhotosByLotIdsAsync(
        IReadOnlyCollection<Guid> lotIds,
        CancellationToken cancellationToken = default);

    Task AddAsync(LotPhoto lotPhoto, CancellationToken cancellationToken = default);
    void Remove(LotPhoto lotPhoto);
}