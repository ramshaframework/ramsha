

namespace Ramsha.Account.Auth.JwtAuth;

public class RamshaJwTValidationOptions
{
    public string SecurityKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public TimeSpan AccessTokenExpiration { get; set; }
}
