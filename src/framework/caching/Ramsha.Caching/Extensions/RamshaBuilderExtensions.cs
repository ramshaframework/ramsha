using Ramsha.Caching;

namespace Ramsha;

public static class RamshaBuilderExtensions
{
    public static RamshaBuilder AddCaching(this RamshaBuilder builder)
    {
        return builder.AddModule<CachingModule>();
    }
}
