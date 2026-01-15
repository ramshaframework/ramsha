using Microsoft.EntityFrameworkCore;
using Ramsha.Common.Domain;
using Ramsha.EntityFrameworkCore;
using Ramsha.JwtAuth.Domain;

namespace Ramsha.JwtAuth.Persistence;


[ConnectionString(RamshaJwtAuthDbContextConstants.ConnectionStringName)]
public class RamshaJwtAuthDbContext(
    DbContextOptions<RamshaJwtAuthDbContext> options
)
: RamshaEFDbContext<RamshaJwtAuthDbContext>(options), IRamshaJwtAuthDbContext
{
    public DbSet<RamshaRefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureJwtAuth();
    }
}
