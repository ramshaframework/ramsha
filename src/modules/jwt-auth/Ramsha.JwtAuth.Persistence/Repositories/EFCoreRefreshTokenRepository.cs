

using Microsoft.EntityFrameworkCore;
using Ramsha.EntityFrameworkCore;
using Ramsha.JwtAuth.Domain;

namespace Ramsha.JwtAuth.Persistence;

public class EFCoreRefreshTokenRepository : EFCoreRepository<IRamshaJwtAuthDbContext, RamshaRefreshToken, long>, IRefreshTokenRepository
{
    public async Task<List<RamshaRefreshToken>> GetAllActiveForUser(string userId)
    {
        return await UnitOfWork(async () =>
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedOn != null && DateTime.UtcNow >= x.ExpiresOn)
            .ToListAsync();
        });
    }
}
