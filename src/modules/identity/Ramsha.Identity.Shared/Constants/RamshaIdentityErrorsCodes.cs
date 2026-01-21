

namespace Ramsha.Identity.Shared;

public static class RamshaIdentityErrorsCodes
{
    public const string IDENTITY_PREFIX = "Identity:";
    public const string Default = IDENTITY_PREFIX + nameof(Default);
    public const string GenerateUsernameErrorCode = IDENTITY_PREFIX + nameof(GenerateUsernameErrorCode);
    public const string RequiresTwoFactor = nameof(RequiresTwoFactor);
    public const string IsNotAllowed = nameof(IsNotAllowed);
    public const string IsLockedOut = nameof(IsLockedOut);

}
