using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Annotations.Core.Entities;

public class User : IdentityUser
{
    [Key]
    public required string UserId {get; set; }
    [Required]
    [StringLength(50)]
    public required string FirstName { get; set; }
    [Required]
    [StringLength(50)]
    public required string LastName { get; set; }
}
