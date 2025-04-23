using Annotations.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API;

public class AnnotationsDbContext : IdentityDbContext<User>
{
    public DbSet<User> Userz { get; set; } // name Users causes issues. Userz is a temporary name
    public DbSet<Admin> Admins { get; set; }
    public DbSet<MedicalProfessional> MedicalProfessionals { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Dataset> Datasets { get; set; }
    public DbSet<VesselPoint> VesselPoints { get; set; }
    public DbSet<VesselSegment> VesselSegments { get; set; }
    public DbSet<VesselTree> VesselTrees { get; set; }

    public AnnotationsDbContext(DbContextOptions<AnnotationsDbContext> options) : base(options)
    {
        Userz = Set<User>();
        Admins = Set<Admin>();
        MedicalProfessionals = Set<MedicalProfessional>();
        Images = Set<Image>();
        Datasets = Set<Dataset>();
        VesselPoints = Set<VesselPoint>();
        VesselSegments = Set<VesselSegment>();
        VesselTrees = Set<VesselTree>();
    }

    /*protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Dataset>().HasMany(d => d.ImageIds).WithMany().HasForeignKey(i => i.ImageId);
	}
    */

}