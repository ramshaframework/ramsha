namespace Ramsha.Localization;


public interface ILocalizationResourceStore
{
    string GetName();

    Task FillAsync(
        Dictionary<string, string> result,
        ResourceDefinition rootResource,
        IReadOnlyList<ResourceDefinition> resourceHierarchy,
        string culture,
        CancellationToken cancellationToken = default);
}

public abstract class LocalizationResourceStore(string name) : ILocalizationResourceStore
{
    public string GetName() => name;
    public abstract Task FillAsync(Dictionary<string, string> result, ResourceDefinition rootResource, IReadOnlyList<ResourceDefinition> resourceHierarchy, string culture, CancellationToken cancellationToken = default);
}

