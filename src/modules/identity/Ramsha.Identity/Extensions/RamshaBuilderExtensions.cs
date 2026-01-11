
using Ramsha.Identity;


namespace Ramsha;

public static class RamshaBuilderExtensions
{
    public static RamshaBuilder AddIdentity(this RamshaBuilder ramsha)
    {
        ramsha.AddModule<IdentityModule>();
        return ramsha;
    }

}
