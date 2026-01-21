using Microsoft.EntityFrameworkCore;
using Ramsha.Common.Domain;
using Ramsha.EntityFrameworkCore;
using Ramsha.Jwt.Domain;

namespace Ramsha.Jwt.Persistence;

[ConnectionString(RamshaJwtDbContextConstants.ConnectionStringName)]
public interface IRamshaJwtDbContext : IRamshaEFDbContext
{
    DbSet<RamshaRefreshToken> RefreshTokens { get; }
}
