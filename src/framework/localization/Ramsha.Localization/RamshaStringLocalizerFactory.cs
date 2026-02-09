using System.Collections.Concurrent;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ramsha.Localization.Abstractions;

namespace Ramsha.Localization;

public class RamshaStringLocalizerFactory(IOptions<RamshaLocalizationOptions> options, ILogger<RamshaStringLocalizer> logger, IEnumerable<ILocalizationResourceProvider> providers) : IStringLocalizerFactory
{
    private readonly ConcurrentDictionary<string, IStringLocalizer> _localizerCache = new();
    public IStringLocalizer Create(Type resourceSource)
    {
        var name = ResourceNameAttribute.GetName(resourceSource);
        var cacheKey = $"{name}";

        return _localizerCache.GetOrAdd(cacheKey, _ =>
            new RamshaStringLocalizer(
                name,
                options.Value,
                providers,
                logger
            ));
    }

    public IStringLocalizer Create(string name, string location)
    {
        var cacheKey = $"{location}.{name}";
        return _localizerCache.GetOrAdd(cacheKey, _ =>
             new RamshaStringLocalizer(
                name,
                options.Value,
                providers,
                logger
               )
        );
    }
}
