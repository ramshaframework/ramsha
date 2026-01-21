

using Ramsha.Account.Auth.ApiAuth;

namespace Ramsha;

public static class RamshaBuilderExtensions
{
    public static RamshaBuilder AddAccountApiAuth(this RamshaBuilder builder)
    {
        builder.AddModule<AccountApiAuthModule>();
        return builder;
    }
}
