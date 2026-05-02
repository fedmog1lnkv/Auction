using MediatR;
using SharedKernel;

namespace Application.Auth.Login;

public sealed record LoginQuery(string Email, string Password) : IRequest<Result<LoginResponse>>;