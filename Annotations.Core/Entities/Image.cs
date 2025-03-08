using System.ComponentModel.DataAnnotations;

namespace Annotations.Core.Entities;

public class Image
{
    public required int Id { get; set; }
    [StringLength(100)]
    public required string Title { get; set; }
    [StringLength(1000)]
    public required string Description { get; set; }
    public required byte[] ImageData { get; set; }

    /*Later add foreign keys:
    UploadedBy (referencing AdminId)
    DeletedBy (referencing AdminId)
    */
}