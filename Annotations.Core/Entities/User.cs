using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Annotations.Core.Entities;

public class User : IdentityUser
{
    // Primary key is provided by ID provider!
    [StringLength(50)]
    public required string FirstName { get; set; }
    [StringLength(50)]
    public required string LastName { get; set; }
}
