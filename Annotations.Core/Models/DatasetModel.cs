using System.Collections;

namespace Annotations.Core.Models;

public class DatasetModel
{
  
    public required int Id { get; set; }

    public required List<int> ImageIds{ get; set; }
   
    public required string Category { get; set; }
   
    public required int AnnotatedBy { get; set; }
 
    public required int ReviewedBy { get; set; }
}