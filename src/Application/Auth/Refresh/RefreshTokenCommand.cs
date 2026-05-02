using MediatR;
using SharedKernel;

namespace Application.Auth.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<RefreshTokenResponse>>;