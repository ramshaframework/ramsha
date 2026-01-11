using Ramsha.Caching.Redis;

namespace Ramsha;

public static class RamshaBuilderExtensions
{
    public static RamshaBuilder AddRedisCaching(this RamshaBuilder builder)
    {
        return builder.AddModule<RedisCachingModule>();
    }
}
