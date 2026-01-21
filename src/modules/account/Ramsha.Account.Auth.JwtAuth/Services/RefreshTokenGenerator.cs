

using System.Security.Cryptography;
using Microsoft.Extensions.Options;


namespace Ramsha.Account.Auth.JwtAuth;

public class RefreshTokenGenerator(
    IOptionsSnapshot<RamshaRefreshTokenOptions> options
    )
{
    public RefreshTokenInfo Generate()
    {
        var randomNumber = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return new RefreshTokenInfo(Convert.ToBase64String(randomNumber), DateTime.UtcNow.Add(options.Value.Expiration));
    }
}
