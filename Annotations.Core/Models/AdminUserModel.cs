namespace Annotations.Core.Models;

public class AdminUserModel
{
    public required int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string? Email { get; set; }
}
