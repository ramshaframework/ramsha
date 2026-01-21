

using Ramsha.Account.Auth.JwtAuth;

namespace Ramsha;

public static class RamshaBuilderExtensions
{
    public static RamshaBuilder AddAccountJwtAuth(this RamshaBuilder builder)
    {
        builder.AddModule<AccountJwtAuthModule>();
        return builder;
    }
}
