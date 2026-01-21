using Microsoft.EntityFrameworkCore;
using Ramsha.Common.Domain;
using Ramsha.EntityFrameworkCore;
using Ramsha.Jwt.Domain;

namespace Ramsha.Jwt.Persistence;


[ConnectionString(RamshaJwtDbContextConstants.ConnectionStringName)]
public class RamshaJwtDbContext(
    DbContextOptions<RamshaJwtDbContext> options
)
: RamshaEFDbContext<RamshaJwtDbContext>(options), IRamshaJwtDbContext
{
    public DbSet<RamshaRefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureJwt();
    }
}
