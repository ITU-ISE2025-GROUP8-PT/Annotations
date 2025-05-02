using Annotations.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API;

/// <summary>
/// Database context for the annotations application backend.
/// </summary>
public class AnnotationsDbContext : IdentityDbContext<User>
{
    /// <summary>
    /// Database set of users. Currently unused.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Database set of images for research use.
    /// </summary>
    public DbSet<Image> Images { get; set; }
    
    /// <summary>
    /// Database set of datasets
    /// </summary>
    public DbSet<Dataset> Datasets { get; set; }
    
    /// <summary>
    /// Database set of annotations upon images
    /// </summary>
    public DbSet<Annotation> Annotation { get; set; }
    
    public AnnotationsDbContext(DbContextOptions<AnnotationsDbContext> options) : base(options)
    {
        Users = Set<User>();
        Images = Set<Image>();
        Datasets = Set<Dataset>();
        Annotation = Set<Annotation>();
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		
		//Foreign key setup between AnnotationType and UserId
		builder.Entity<Annotation>()
			.HasOne<User>()
			.WithMany()
			.HasForeignKey("MpId");

		//Foreign key setup between AnnotationType and ImageId
		builder.Entity<Annotation>()
			.HasOne<Image>()
			.WithMany()
			.HasForeignKey("ImgId");
		
	}
}