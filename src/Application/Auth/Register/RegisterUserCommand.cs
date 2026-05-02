using MediatR;
using SharedKernel;

namespace Application.Auth.Register;

public sealed record RegisterUserCommand(string Email, string FirstName, string LastName, string Password)
    : IRequest<Result<RegisterResponse>>;