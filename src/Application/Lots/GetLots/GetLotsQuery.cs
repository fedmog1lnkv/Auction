using Application.Common.Pagination;
using Domain.Lots;
using MediatR;
using SharedKernel;

namespace Application.Lots.GetLots;

public sealed record GetLotsQuery(
    LotStatus? Status,
    Guid? SellerId,
    int Page = 1,
    int Limit = 20) : IRequest<Result<PagedResponse<LotResponse>>>;