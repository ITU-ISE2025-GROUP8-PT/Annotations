using Annotations.Core.Models;
using Microsoft.EntityFrameworkCore;


namespace Annotations.API.Services;


/// <summary>
/// Defines a service for accessing user data.
/// </summary>
public interface IUserService
{
    Task<List<UserModel>> GetUsers();
}



public class UserService : IUserService
{
    private AnnotationsDbContext _context;



    /// <summary>
    /// Constructor of the service class. 
    /// </summary>
    /// <param name="context"> Annotations database context containing the user data. </param>
    public UserService(AnnotationsDbContext context)
    {
        _context = context;
    }

    
    
    /// <summary>
    /// Retrieves all users. 
    /// </summary>
    /// <returns> A list of all AdminUserModel. </returns>
    public async Task<List<UserModel>> GetUsers()
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
