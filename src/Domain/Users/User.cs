using SharedKernel;

namespace Domain.Users;

public sealed class User : Entity
{
    private User()
    {
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    public static Result<User> Create(Guid id, string email, string firstName, string lastName, string passwordHash)
    {
        if (!IsValidEmail(email))
            return Result.Failure<User>(UserErrors.InvalidEmail);

        if (string.IsNullOrWhiteSpace(firstName) || firstName.Trim().Length > 100)
            return Result.Failure<User>(UserErrors.InvalidFirstName);

        if (string.IsNullOrWhiteSpace(lastName) || lastName.Trim().Length > 100)
            return Result.Failure<User>(UserErrors.InvalidLastName);

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result.Failure<User>(UserErrors.InvalidPasswordHash);

        var user = new User
        {
            Id = id,
            Email = email.Trim(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            PasswordHash = passwordHash
        };

        return Result.Success(user);
    }

    public Result ChangePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result.Failure(UserErrors.InvalidPasswordHash);

        PasswordHash = passwordHash;
        return Result.Success();
    }

    public Result UpdateProfile(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Trim().Length > 100)
            return Result.Failure(UserErrors.InvalidFirstName);

        if (string.IsNullOrWhiteSpace(lastName) || lastName.Trim().Length > 100)
            return Result.Failure(UserErrors.InvalidLastName);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        return Result.Success();
    }

    public static User CreateSeed(Guid id, string email, string firstName, string lastName, string passwordHash)
    {
        var result = Create(id, email, firstName, lastName, passwordHash);
        return result.IsFailure
            ? throw new InvalidOperationException($"Invalid seed user: {result.Error.Code}")
            : result.Value;
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var normalized = email.Trim();
        var atIndex = normalized.IndexOf('@');
        return atIndex > 0 && atIndex < normalized.Length - 1;
    }
}