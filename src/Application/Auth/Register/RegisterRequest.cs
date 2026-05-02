namespace Application.Auth.Register;

public sealed record RegisterRequest(string Email, string FirstName, string LastName, string Password);