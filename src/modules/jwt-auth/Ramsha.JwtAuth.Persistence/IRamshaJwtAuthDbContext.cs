using Microsoft.EntityFrameworkCore;
using Ramsha.Common.Domain;
using Ramsha.EntityFrameworkCore;
using Ramsha.JwtAuth.Domain;

namespace Ramsha.JwtAuth.Persistence;

[ConnectionString(RamshaJwtAuthDbContextConstants.ConnectionStringName)]
public interface IRamshaJwtAuthDbContext : IRamshaEFDbContext
{
    DbSet<RamshaRefreshToken> RefreshTokens { get; }
}
