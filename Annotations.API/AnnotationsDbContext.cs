using Annotations.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API;

public class AnnotationsDbContext : IdentityDbContext<User>
{
    

    public AnnotationsDbContext(DbContextOptions<AnnotationsDbContext> options) : base(options)
    {
        
    }
}
