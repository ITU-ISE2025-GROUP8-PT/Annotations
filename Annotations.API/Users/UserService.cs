using Annotations.Core.Entities;
using System.Security.Claims;

namespace Annotations.API.Users;

/// <summary>
/// Defines a scoped service for user information stored in the backend API database.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// <para>Finds the users own database entry by their claims principal.</para>
    /// <para>Returns <c>null</c> if user is not recorded in the database.</para>
    /// </summary>
    /// <param name="claimsPrincipal">Claims principal from authenticated http context.</param>
    Task<User?> TryFindUserAsync(ClaimsPrincipal claimsPrincipal);

    /// <summary>
    /// <para>Creates a database entry for the user based on their claims principal.</para>
    /// <para>An <c>InvalidOperationException</c> is thrown if user is already recorded in the database.</para>
    /// </summary>
    /// <param name="claimsPrincipal">Claims principal from authenticated http context.</param>
    /// <return>The newly created database entry for the user.</return>
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
        if (await TryFindUserAsync(claimsPrincipal) != null)
        {
            throw new InvalidOperationException("This user already exists");
        }

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
