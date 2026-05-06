using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Lots;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.Lots.DeleteLot;

public sealed class DeleteLotCommandHandler(
    ILotRepository lotRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<DeleteLotCommand, Result>
{
    public async Task<Result> Handle(DeleteLotCommand command, CancellationToken cancellationToken)
    {
        var lot = await lotRepository.GetByIdAsync(command.Id, cancellationToken);
        if (lot is null)
            return Result.Failure(LotErrors.NotFound);

        if (!lot.CanBeManagedBy(userContext.UserId))
            return Result.Failure(LotErrors.Forbidden);

        var canDelete = lot.EnsureCanBeDeleted();
        if (canDelete.IsFailure) return canDelete;

        lotRepository.Remove(lot);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}