namespace Ramsha.Localization;


public interface ILocalizationResourceStore
{
    string Name { get; }

    Task FillAsync(
        Dictionary<string, string> result,
        ResourceDefinition rootResource,
        IReadOnlyList<ResourceDefinition> resourceHierarchy,
        string culture,
        CancellationToken cancellationToken = default);
}

