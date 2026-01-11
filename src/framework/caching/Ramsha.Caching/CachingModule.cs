using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace Ramsha.Caching;

public class CachingModule : RamshaModule
{
    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);

        var ramshaOptions = context.Services.ExecutePreparedOptions<RamshaCachingOptions>();

        context.Services.AddHybridCache(options =>
        {
            options.MaximumKeyLength = ramshaOptions.MaximumKeyLength;
            options.MaximumPayloadBytes = ramshaOptions.MaximumPayloadBytes;
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = ramshaOptions.DefaultEntryOptions.Expiration,
                LocalCacheExpiration = ramshaOptions.DefaultEntryOptions.LocalCacheExpiration
            };
        
        });

        context.Services.AddSingleton<IRamshaCache, RamshaCache>();
    }
}
