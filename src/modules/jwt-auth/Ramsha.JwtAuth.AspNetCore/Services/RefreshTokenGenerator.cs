

using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Ramsha.JwtAuth.Domain;
using Ramsha.JwtAuth.Shared;

namespace Ramsha.JwtAuth.AspNetCore;

public class RefreshTokenGenerator(
    IOptionsSnapshot<RamshaRefreshTokenOptions> options
    ) : IRefreshTokenGenerator
{
    public RefreshTokenInfo Generate()
    {
        var randomNumber = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return new RefreshTokenInfo(Convert.ToBase64String(randomNumber), DateTime.UtcNow.Add(options.Value.Expiration));
    }
}
