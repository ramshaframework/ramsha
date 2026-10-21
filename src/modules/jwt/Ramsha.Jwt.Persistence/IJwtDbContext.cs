using Microsoft.EntityFrameworkCore;
using Ramsha.Common.Domain;
using Ramsha.EntityFrameworkCore;
using Ramsha.Jwt.Domain;

namespace Ramsha.Jwt.Persistence;

[ConnectionString(JwtDbContextConstants.ConnectionStringName)]
public interface IJwtDbContext : IRamshaEFDbContext
{
    DbSet<RamshaRefreshToken> RefreshTokens { get; }
}
