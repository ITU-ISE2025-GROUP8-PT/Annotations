namespace Annotations.Core.Models;

public class AnnotationModel
{
     public required int Id { get; set; }
     public required int UserId { get; set; } //Id of the user, that made the Annotation
     public required int ImgId { get; set; } //Id of annotated image
     public required VesselAnnotationModel AnnotationTree { get; set; }
   
}