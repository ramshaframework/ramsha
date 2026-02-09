namespace Ramsha.Localization;


public interface ILocalizationResourceProvider
{
    string Name { get; }

    Task<IDictionary<string, string>> LoadAsync(
        ResourceDefinition resource,
        string culture,
        CancellationToken cancellationToken = default);
}

