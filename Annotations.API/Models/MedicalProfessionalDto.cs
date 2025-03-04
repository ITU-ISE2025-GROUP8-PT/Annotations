namespace Annotations.API.Models;

public class MedicalProfessionalDto
{
    public required int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string? Email { get; set; }
    public required string Affiliation  { get; set; } // the place where the medical professional works
    public required string JobTitle  { get; set; }
    public required int TotalAssignmentsFinished  { get; set; }
    public required int ProfilePictureID  { get; set; }
}