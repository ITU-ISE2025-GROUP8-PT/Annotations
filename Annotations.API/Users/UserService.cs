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
    public Task<User> CreateUser(ClaimsPrincipal claimsPrincipal)
    {
        throw new NotImplementedException();
    }

    public Task<User?> TryFindUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        throw new NotImplementedException();
    }
}
