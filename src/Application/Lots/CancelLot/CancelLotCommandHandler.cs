using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Lots;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.Lots.CancelLot;

public sealed class CancelLotCommandHandler(
    ILotRepository lotRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<CancelLotCommand, Result<LotResponse>>
{
    public async Task<Result<LotResponse>> Handle(CancelLotCommand command, CancellationToken cancellationToken)
    {
        var lot = await lotRepository.GetByIdAsync(command.Id, cancellationToken);
        if (lot is null)
            return Result.Failure<LotResponse>(LotErrors.NotFound);

        if (!lot.CanBeManagedBy(userContext.UserId))
            return Result.Failure<LotResponse>(LotErrors.Forbidden);

        var cancelResult = lot.Cancel(DateTime.UtcNow);
        if (cancelResult.IsFailure)
            return Result.Failure<LotResponse>(cancelResult.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(LotResponse.FromDomain(lot));
    }
}