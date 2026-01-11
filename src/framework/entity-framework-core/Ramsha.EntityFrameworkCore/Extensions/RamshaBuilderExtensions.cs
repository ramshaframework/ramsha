

using Ramsha.EntityFrameworkCore;

namespace Ramsha;

public static class RamshaBuilderExtensions
{
    public static RamshaBuilder AddEntityFrameworkCore(this RamshaBuilder ramsha)
    {
        ramsha.AddModule<EntityFrameworkCoreModule>();
        return ramsha;
    }
}
