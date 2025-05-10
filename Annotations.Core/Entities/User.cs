using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Annotations.Core.Entities;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User
{
    /*
     * Attention: 
     * In general, unless the SSO is known to impose string length limits,
     * do not try to impose those limits here. Capabilities for essential 
     * properties like UserId, UserName, Email, Phone, etc. should match 
     * with the SSO provider. 
     */

    /// <summary>
    /// User ID matching that given by SSO provider. 
    /// </summary>
    [Key]
    public required string UserId {get; set; }

    /// <summary>
    /// The display name of the user. (Handle)
    /// </summary>
    [Required]
    public required string UserName { get; set; }
}
