namespace Ramsha.Account.Auth.JwtAuth;

public class RamshaJwtRefreshRequest
{
    public string? RefreshToken { get; set; }
    public string? OldAccessToken { get; set; }
}
