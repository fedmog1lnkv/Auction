using Application.Abstractions.Users;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users;

internal sealed class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        dbContext.Users.FirstOrDefaultAsync(
            user => user.Email == email,
            cancellationToken);

    public Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        dbContext.Users.AddAsync(user, cancellationToken).AsTask();
}