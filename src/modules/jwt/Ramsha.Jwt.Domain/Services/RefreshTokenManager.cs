using Ramsha.Common.Domain;

namespace Ramsha.Jwt.Domain;

public class RefreshTokenManager(
    IRefreshTokenRepository repository) : RamshaDomainManager
{
    public Task RevokeAsync(RamshaRefreshToken refreshToken)
    {
        refreshToken.RevokedOn = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public async Task<RamshaRefreshToken> CreateAsync(string userId, string token, DateTime tokenExpired)
    {
        var refreshToken = new RamshaRefreshToken
        {
            Token = token,
            ExpiresOn = tokenExpired,
            CreatedOn = DateTime.UtcNow,
            UserId = userId
        };

        await repository.AddAsync(refreshToken);

        return refreshToken;
    }


}

