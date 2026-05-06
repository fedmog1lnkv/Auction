using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Lots;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.Lots.UpdateLot;

public sealed class UpdateLotCommandHandler(
    ILotRepository lotRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<UpdateLotCommand, Result<LotResponse>>
{
    public async Task<Result<LotResponse>> Handle(UpdateLotCommand command, CancellationToken cancellationToken)
    {
        var lot = await lotRepository.GetByIdAsync(command.Id, cancellationToken);
        if (lot is null)
            return Result.Failure<LotResponse>(LotErrors.NotFound);

        if (!lot.CanBeManagedBy(userContext.UserId))
            return Result.Failure<LotResponse>(LotErrors.Forbidden);

        var title = command.Title ?? lot.Title;
        var description = command.Description ?? lot.Description;
        var startingPrice = command.StartingPrice ?? lot.StartingPrice;
        var minBidStep = command.MinBidStep ?? lot.MinBidStep;
        var startsAt = command.StartsAt ?? lot.StartsAt;
        var endsAt = command.EndsAt ?? lot.EndsAt;

        var updateResult = lot.UpdateDraft(
            title,
            description,
            startingPrice,
            minBidStep,
            startsAt,
            endsAt,
            DateTime.UtcNow);

        if (updateResult.IsFailure)
            return Result.Failure<LotResponse>(updateResult.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(LotResponse.FromDomain(lot));
    }
}