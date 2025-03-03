namespace Annotations.Core.Entities;

public class MedicalProfessional : User 
{
    public required string Affiliation  { get; set; } // the place where the medical professional works
    public required string JobTitle  { get; set; }
    public required int TotalAssignmentsFinished  { get; set; }
    public required int ProfilePictureID  { get; set; }
    
}