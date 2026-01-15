using Ramsha.Common.Domain;

namespace Ramsha.JwtAuth.Domain;

public class RefreshTokenManager(
    IRefreshTokenRepository repository,
    IRefreshTokenGenerator tokenGenerator) : RamshaDomainManager
{
    public Task RevokeAsync(RamshaRefreshToken refreshToken)
    {
        refreshToken.RevokedOn = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public async Task<RamshaRefreshToken> GenerateAsync(string userId)
    {
        var (token, expired) = tokenGenerator.Generate();
        var refreshToken = new RamshaRefreshToken
        {
            Token = token,
            ExpiresOn = expired,
            CreatedOn = DateTime.UtcNow,
            UserId = userId
        };

        await repository.AddAsync(refreshToken);

        return refreshToken;
    }


}

