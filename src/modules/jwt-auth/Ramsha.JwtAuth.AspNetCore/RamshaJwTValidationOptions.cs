

namespace Ramsha.JwtAuth.AspNetCore;

public class RamshaJwTValidationOptions
{
    public string SecurityKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public double DurationInMinutes { get; set; } = 2;
}
