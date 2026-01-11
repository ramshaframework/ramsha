

using Ramsha.Account;

namespace Ramsha;

public static class RamshaBuilderExtensions
{
    public static RamshaBuilder AddAccount(this RamshaBuilder builder)
    {
        builder.AddModule<AccountModule>();
        return builder;
    }
}
