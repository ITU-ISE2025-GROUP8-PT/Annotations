using Annotations.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Annotations.Core;

public class AnnotationsDbContext : IdentityDbContext<User>
{
    public DbSet<User> Users { get; set; } // an issue might occur with the naming Users
    public DbSet<Admin> Admins { get; set; }
    public DbSet<MedicalProfessional> MedicalProfessionals { get; set; }

    public AnnotationsDbContext(DbContextOptions<AnnotationsDbContext> options) : base(options)
    {
        Users = Set<User>();
        Admins = Set<Admin>();
        MedicalProfessionals = Set<MedicalProfessional>();
    }
}