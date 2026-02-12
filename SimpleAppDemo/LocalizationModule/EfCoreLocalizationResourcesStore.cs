using System;
using Ramsha.Localization;

namespace SimpleAppDemo;

public class EfCoreLocalizationResourcesStore(ILogger<EfCoreLocalizationResourcesStore> logger) : ILocalizationResourceStore
{
    public string Name => "ef";

    public Task FillAsync(Dictionary<string, string> result,
        ResourceDefinition rootResource,
        IReadOnlyList<ResourceDefinition> resourceHierarchy, string culture, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("EfCoreLocalizationResourcesStore returning empty dic");
        return Task.FromResult<IDictionary<string, string>>(new Dictionary<string, string>());
    }
}
