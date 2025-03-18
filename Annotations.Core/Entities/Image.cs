using System.ComponentModel.DataAnnotations;

namespace Annotations.Core.Entities;

public class Image
{
    
	[Required]
	public required int Id { get; set; }
	[Required]
    [StringLength(100)]
    public required string Title { get; set; }
	[Required]
    [StringLength(1000)]
    public required string Description { get; set; }
	[Required]
    public required byte[] ImageData { get; set; }
}

    /*Later add foreign keys:
    UploadedBy (referencing AdminId)
    DeletedBy (referencing AdminId)
    */