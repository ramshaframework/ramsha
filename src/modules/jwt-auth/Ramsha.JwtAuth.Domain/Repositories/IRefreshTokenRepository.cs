using Ramsha.Common.Domain;

namespace Ramsha.JwtAuth.Domain;

public interface IRefreshTokenRepository : IRepository<RamshaRefreshToken, long>
{
    Task<List<RamshaRefreshToken>> GetAllActiveForUser(string userId);
}
