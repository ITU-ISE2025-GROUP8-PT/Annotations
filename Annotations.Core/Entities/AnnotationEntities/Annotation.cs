using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Annotations.Core.VesselObjects;

namespace Annotations.Core.Entities;

public class Annotation
{
    [Required]
    public required int Id { get; set; }

    [ForeignKey("User")]
    [Required]
    public required string UserId { get; set; } //Id of the user, that made the Annotation
    
    [ForeignKey("Image")]
    [Required]
    public required int ImgId { get; set; } //Id of annotated image
    
    [Required]
    public required VesselAnnotation AnnotationTree { get; set; }
}