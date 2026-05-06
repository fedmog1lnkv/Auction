using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Lots;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.Lots.StartLot;

public sealed class StartLotCommandHandler(
    ILotRepository lotRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<StartLotCommand, Result<LotResponse>>
{
    public async Task<Result<LotResponse>> Handle(StartLotCommand command, CancellationToken cancellationToken)
    {
        var lot = await lotRepository.GetByIdAsync(command.Id, cancellationToken);
        if (lot is null)
            return Result.Failure<LotResponse>(LotErrors.NotFound);

        if (!lot.CanBeManagedBy(userContext.UserId))
            return Result.Failure<LotResponse>(LotErrors.Forbidden);

        var startResult = lot.Start(DateTime.UtcNow);
        if (startResult.IsFailure)
            return Result.Failure<LotResponse>(startResult.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(LotResponse.FromDomain(lot));
    }
}