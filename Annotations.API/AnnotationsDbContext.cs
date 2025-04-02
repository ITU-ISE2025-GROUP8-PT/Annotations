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

    public AnnotationsDbContext(DbContextOptions<AnnotationsDbContext> options) : base(options)
    {
        Userz = Set<User>();
        Admins = Set<Admin>();
        MedicalProfessionals = Set<MedicalProfessional>();
        Images = Set<Image>();
        Datasets = Set<Dataset>();
    }
}