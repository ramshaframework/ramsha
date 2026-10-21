using Microsoft.EntityFrameworkCore;
using Ramsha.Common.Domain;
using Ramsha.EntityFrameworkCore;
using Ramsha.Jwt.Domain;

namespace Ramsha.Jwt.Persistence;


[ConnectionString(JwtDbContextConstants.ConnectionStringName)]
public class JwtDbContext(
    DbContextOptions<JwtDbContext> options
)
: RamshaEFDbContext<JwtDbContext>(options), IJwtDbContext
{
    public DbSet<RamshaRefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureJwt();
    }
}
