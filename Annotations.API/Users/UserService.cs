using Annotations.Core.Entities;
using System.Security.Claims;

namespace Annotations.API.Users;

public interface IUserService
{
    Task<User?> TryFindUserAsync(ClaimsPrincipal claimsPrincipal);
    Task<User> CreateUser(ClaimsPrincipal claimsPrincipal);
}

public class UserService : IUserService
{
    private readonly AnnotationsDbContext _dbContext;

    public UserService(AnnotationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> CreateUser(ClaimsPrincipal claimsPrincipal)
    {
        var newUser = new User
        {
            UserId      = claimsPrincipal.FindFirstValue("sub") ?? throw new ArgumentNullException(nameof(claimsPrincipal)),
            UserName    = claimsPrincipal.Identity!.Name ?? throw new ArgumentNullException(nameof(claimsPrincipal)),
        };

        await _dbContext.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();
        
        return newUser;
    }

    public async Task<User?> TryFindUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        return await _dbContext.Users.FindAsync(claimsPrincipal.FindFirstValue("sub"));
    }
}
