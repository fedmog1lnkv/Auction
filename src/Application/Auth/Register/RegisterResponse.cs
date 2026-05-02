namespace Application.Auth.Register;

public sealed record RegisterResponse(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken);