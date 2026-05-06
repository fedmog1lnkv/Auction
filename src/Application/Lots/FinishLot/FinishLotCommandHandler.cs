using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Lots;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.Lots.FinishLot;

public sealed class FinishLotCommandHandler(
    ILotRepository lotRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<FinishLotCommand, Result<LotResponse>>
{
    public async Task<Result<LotResponse>> Handle(FinishLotCommand command, CancellationToken cancellationToken)
    {
        var lot = await lotRepository.GetByIdAsync(command.Id, cancellationToken);
        if (lot is null)
            return Result.Failure<LotResponse>(LotErrors.NotFound);

        if (!lot.CanBeManagedBy(userContext.UserId))
            return Result.Failure<LotResponse>(LotErrors.Forbidden);

        var finishResult = lot.Finish(DateTime.UtcNow);
        if (finishResult.IsFailure)
            return Result.Failure<LotResponse>(finishResult.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(LotResponse.FromDomain(lot));
    }
}