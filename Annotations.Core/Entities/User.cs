using Microsoft.AspNetCore.Identity;

namespace Annotations.Core.Entities;

public class User : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    
    
}