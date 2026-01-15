namespace Ramsha.JwtAuth.Shared;

public class RamshaJwtRefreshRequest
{
    public string? RefreshToken { get; set; }
    public string? OldAccessToken { get; set; }
}
