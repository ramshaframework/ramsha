using System;
using Ramsha.Common.Domain;
using Ramsha.Localization;
using SimpleAppDemo.LocalizationModule;

namespace SimpleAppDemo;

public sealed class EfCoreLocalizationResourcesStore(IRepository<LocalizationText> repository)
    : LocalizationResourceStore("db")
{
    public override async Task FillAsync(
        Dictionary<string, string> result,
        ResourceDefinition rootResource,
        IReadOnlyList<ResourceDefinition> resourceHierarchy,
        string culture,
        CancellationToken cancellationToken = default)
    {
        var resourceNames = resourceHierarchy
            .Select(r => r.Name)
            .ToList();

        var texts = await repository
            .GetListAsync(x =>
                resourceNames.Contains(x.ResourceName) &&
                x.Culture == culture);

        foreach (var text in texts)
        {
            result[text.Key] = text.Value;
        }
    }
}
