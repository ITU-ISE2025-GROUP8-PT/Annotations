using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Annotations.Core.Entities;

public class User : IdentityUser
{
    [Key]
    public required int UserId {get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    
    
}
