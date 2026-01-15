using Microsoft.EntityFrameworkCore;
using Ramsha.EntityFrameworkCore;
using Ramsha.Identity.Persistence;
using Ramsha.JwtAuth.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
: RamshaEFDbContext<AppDbContext>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder
        .ConfigureIdentity()
        .ConfigureJwtAuth();
    }
}
