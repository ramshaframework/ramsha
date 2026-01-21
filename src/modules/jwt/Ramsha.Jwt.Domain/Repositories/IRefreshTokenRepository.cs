using Ramsha.Common.Domain;

namespace Ramsha.Jwt.Domain;

public interface IRefreshTokenRepository : IRepository<RamshaRefreshToken, long>
{
    Task<List<RamshaRefreshToken>> GetAllActiveForUser(string userId);
}
