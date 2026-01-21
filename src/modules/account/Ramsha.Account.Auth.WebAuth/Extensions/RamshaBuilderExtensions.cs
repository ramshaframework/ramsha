

using Ramsha.Account.Auth.WebAuth;

namespace Ramsha;

public static class RamshaBuilderExtensions
{
    public static RamshaBuilder AddAccountWebAuth(this RamshaBuilder builder)
    {
        builder.AddModule<AccountWebAuthModule>();
        return builder;
    }
}
