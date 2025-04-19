using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Annotations.Core.Entities;

/// <summary>
/// <para>User entity for the purpose of storing user information for app specific purposes.</para>
/// </summary>
public class User
{
    /*
     * Attention: 
     * In general, unless the SSO is known to impose string length limits,
     * do not try to impose those limits here. Essential properties like 
     * UserId, UserName, Email, Phone, etc. should match SSO provider. 
     */ 
    [Key]
    public required string UserId {get; set; } // User ID matching that given by SSO provider. 

    [Required]
    public required string UserName { get; set; } // User name for display purposes. 
}
