using System.ComponentModel.DataAnnotations;

namespace Annotations.Core.Entities;

public class Dataset
{
    [Required]
    public required int Id { get; set; }
    [Required]
    public required int[] ImageIds{ get; set; }
    [Required]
    public required string Category { get; set; }
    [Required]
    public required int AnnotatedBy { get; set; }
    [Required]
    public required int ReviewedBy { get; set; }
}