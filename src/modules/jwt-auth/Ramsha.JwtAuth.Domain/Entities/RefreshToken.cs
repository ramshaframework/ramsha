using Ramsha.Common.Domain;

namespace Ramsha.JwtAuth.Domain;

public class RamshaRefreshToken
: Entity<long>
{
    public string UserId { get; internal set; }
    public string Token { get; internal set; }
    public DateTime ExpiresOn { get; internal set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
    public DateTime CreatedOn { get; internal set; }
    public DateTime? RevokedOn { get; internal set; }
    public bool IsRevoked => RevokedOn is not null;
    public bool IsActive => !IsRevoked && !IsExpired;
}

