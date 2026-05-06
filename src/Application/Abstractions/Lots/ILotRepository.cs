using Domain.Lots;

namespace Application.Abstractions.Lots;

public interface ILotRepository
{
    Task AddAsync(Lot lot, CancellationToken cancellationToken = default);
    Task<Lot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Lot>> GetListAsync(
        LotStatus? status,
        Guid? sellerId,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(LotStatus? status, Guid? sellerId, CancellationToken cancellationToken = default);
    void Remove(Lot lot);
}