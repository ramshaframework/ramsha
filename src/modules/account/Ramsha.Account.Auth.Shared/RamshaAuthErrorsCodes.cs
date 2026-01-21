

namespace Ramsha.Auth.Shared;

public static class RamshaAccountAuthErrorsCodes
{
    public const string ACCOUNT_AUTH_PREFIX = "AccountAuth:";
    public const string Default = ACCOUNT_AUTH_PREFIX + nameof(Default);
    public const string RequiresTwoFactor = ACCOUNT_AUTH_PREFIX + nameof(RequiresTwoFactor);
    public const string IsNotAllowed = ACCOUNT_AUTH_PREFIX + nameof(IsNotAllowed);
    public const string IsLockedOut = ACCOUNT_AUTH_PREFIX + nameof(IsLockedOut);

}
