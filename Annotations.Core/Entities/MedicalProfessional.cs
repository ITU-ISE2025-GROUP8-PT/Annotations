using System.ComponentModel.DataAnnotations;

namespace Annotations.Core.Entities;

public class MedicalProfessional : User 
{
    [Required]
    [StringLength(50)]
    public required string Affiliation  { get; set; } // the place where the medical professional works
    [Required]
    [StringLength(50)]
    public required string JobTitle  { get; set; }
    [Required]
    public required int TotalAssignmentsFinished  { get; set; }
    [Required]
    [StringLength(50)]
    public required int ProfilePictureID  { get; set; }
    
}
