using System.Security.Claims;
using Annotations.Core.Entities;
using Annotations.Core.Models;
using Microsoft.EntityFrameworkCore;


namespace Annotations.API.Services.Users;


/// <summary>
/// Defines a scoped service for user information stored in the backend API database.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves data on all users. 
    /// </summary>
    Task<List<UserModel>> GetUsersAsync();

    /// <summary>
    /// Tries to find the user in the database by their claims principal.
    /// </summary>
    Task<User?> TryFindUserAsync(ClaimsPrincipal claimsPrincipal);

    /// <summary>
    /// Creates a new user in the database based on their claims principal.
    /// </summary>
    Task<User> CreateUserAsync(ClaimsPrincipal claimsPrincipal);
}





/// <summary>
/// Implements a service for user information stored in the backend API database.
/// </summary>
public class UserService : IUserService
{
    private readonly AnnotationsDbContext _context;


    /// <summary>
    /// Constructor of the service class. 
    /// </summary>
    /// <param name="context"> Annotations database context containing the user data. </param>
    public UserService(AnnotationsDbContext context)
    {
        _context = context;
    }





    /// <summary>
    /// Creates a new user in the database based on their claims principal.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<User> CreateUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        if (await TryFindUserAsync(claimsPrincipal) != null)
        {
            throw new InvalidOperationException("This user already exists");
        }

        var newUser = new User
        {
            UserId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new ArgumentNullException("user id not found in claim principal"),
            UserName = claimsPrincipal.Identity!.Name ?? throw new ArgumentNullException("user name not found in claims principal")
        };

        await _context.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return newUser;
    }





    /// <summary>
    /// Tries to find the user in the database by their claims principal.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public async Task<User?> TryFindUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        return await _context.Users
            .Where(user => user.UserId == claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier))
            .SingleOrDefaultAsync();
    }





    /// <summary>
    /// Retrieves data on all users. 
    /// </summary>
    /// <returns> A list of all AdminUserModel. </returns>
    public async Task<List<UserModel>> GetUsersAsync()
    {
        var users = await _context.Users
            .Select(u => new UserModel
            {
                UserName = u.UserName,
            })
            .ToListAsync();
        return users;
    }
}
