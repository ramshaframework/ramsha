

namespace Ramsha.Account.Auth.JwtAuth;

public class RamshaJwtAuthResponse
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string[] Role { get; set; }
    public bool IsVerified { get; set; }
    public string AccessToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
}
