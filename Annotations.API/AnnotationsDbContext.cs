using Annotations.Core.Entities;
using Annotations.Core.VesselObjects;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API;

/// <summary>
/// Database context for the annotations application backend.
/// </summary>
public class AnnotationsDbContext : DbContext
{
    /// <summary>
    /// Database set of users.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Database set of medical research images.
    /// </summary>
    public DbSet<Image> Images { get; set; }
    
    /// <summary>
    /// Database set of datasets. 
    /// </summary>
    public DbSet<Dataset> Datasets { get; set; }
    
    /// <summary>
    /// Database set of annotations upon images
    /// </summary>
    public DbSet<Annotation> Annotation { get; set; }
    public DbSet<VesselAnnotation> VesselAnnotation { get; set; }
    public DbSet<VesselPoint> VesselPoint { get; set; }
    public DbSet<VesselSegment> VesselSegment { get; set; }
    public AnnotationsDbContext(DbContextOptions<AnnotationsDbContext> options) : base(options)
    {
        Users = Set<User>();
        Images = Set<Image>();
        Datasets = Set<Dataset>();
        Annotation = Set<Annotation>();
        VesselAnnotation = Set<VesselAnnotation>();
        VesselPoint = Set<VesselPoint>();
        VesselSegment = Set<VesselSegment>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

        builder.Entity<Dataset>()
            .HasMany<Image>(ds => ds.Images)
            .WithMany(img => img.Datasets)
            .UsingEntity<DatasetEntry>();

        //Foreign key setup between AnnotationType and UserId
        builder.Entity<Annotation>()
			.HasOne<User>()
			.WithMany()
			.HasForeignKey(u => u.UserId);

		//Foreign key setup between AnnotationType and ImageId
		builder.Entity<Annotation>()
			.HasOne<Image>()
			.WithMany()
			.HasForeignKey("ImgId");
		
		//How to tell EF Core that VesselType AnnotationTree is not a DB entity
		builder.Entity<Annotation>().HasOne<VesselAnnotation>();
	}
}