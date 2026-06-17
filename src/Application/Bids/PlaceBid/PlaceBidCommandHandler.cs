using Application.Abstractions.Authentication;
using Application.Abstractions.Bids;
using MediatR;
using SharedKernel;

namespace Application.Bids.PlaceBid;

public sealed class PlaceBidCommandHandler(
    IBidRepository bidRepository,
    IUserContext userContext) : IRequestHandler<PlaceBidCommand, Result<BidResponse>>
{
    public async Task<Result<BidResponse>> Handle(PlaceBidCommand command, CancellationToken cancellationToken)
    {
        var result = await bidRepository.PlaceAsync(
            command.LotId,
            userContext.UserId,
            command.Amount,
            cancellationToken);

        return result.IsSuccess
            ? Result.Success(BidResponse.FromDomain(result.Value.Bid, result.Value.Lot))
            : Result.Failure<BidResponse>(result.Error);
    }
}