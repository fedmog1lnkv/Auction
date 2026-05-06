using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Lots;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.Lots.CreateLot;

public sealed class CreateLotCommandHandler(
    ILotRepository lotRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<CreateLotCommand, Result<LotResponse>>
{
    public async Task<Result<LotResponse>> Handle(CreateLotCommand command, CancellationToken cancellationToken)
    {
        var createResult = Lot.Create(
            Guid.NewGuid(),
            userContext.UserId,
            command.Title,
            command.Description,
            command.StartingPrice,
            command.MinBidStep,
            command.StartsAt,
            command.EndsAt,
            DateTime.UtcNow);

        if (createResult.IsFailure)
            return Result.Failure<LotResponse>(createResult.Error);

        await lotRepository.AddAsync(createResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(LotResponse.FromDomain(createResult.Value));
    }
}