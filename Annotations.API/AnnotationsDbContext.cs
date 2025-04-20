using Annotations.Core.Entities;
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
    /// Database set of images for research use.
    /// </summary>
    public DbSet<Image> Images { get; set; }

    /// <summary>
    /// Database set of image series.
    /// </summary>
    public DbSet<ImageSeries> ImageSeries { get; set; }
    
    public AnnotationsDbContext(DbContextOptions<AnnotationsDbContext> options) : base(options)
    {
        Users = Set<User>();
        Images = Set<Image>();
        ImageSeries = Set<ImageSeries>();
    }
}
