namespace Application.Auth.Login;

public sealed record LoginResponse(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken);