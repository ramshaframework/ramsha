

using Microsoft.AspNetCore.Http;

namespace Ramsha.JwtAuth.AspNetCore;

public class RamshaRefreshTokenOptions
{
    public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(30);
    public RefreshTokenCookieOptions CookieOptions { get; set; } = new();
}

public class RefreshTokenCookieOptions
{
    public SameSiteMode SameSite { get; set; } = SameSiteMode.Lax;
    public bool Secure { get; set; } = true;
}


