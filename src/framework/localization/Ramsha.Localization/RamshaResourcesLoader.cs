using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ramsha.Caching;

namespace Ramsha.Localization;


public interface IRamshaResourcesLoader
{
    Task<Dictionary<string, string>> LoadAsync(
        ResourceDefinition resource,
        CultureInfo culture,
        bool includeParents);
}

public class RamshaResourcesLoader : IRamshaResourcesLoader
{
    public const string LocalizationResourcesCacheTag = "localizationResources";
    public const string ResourceTagPrefix = "resource_";
    public const string CultureTagPrefix = "culture_";
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRamshaCache _cache;
    private readonly RamshaLocalizationOptions _options;
    public RamshaResourcesLoader(
        IOptions<RamshaLocalizationOptions> options,
        IRamshaCache cache,
        ILocalizationLanguagesProvider localizationLanguages,
         IServiceScopeFactory scopeFactory)
    {
        _options = options.Value;
        _cache = cache;
        _scopeFactory = scopeFactory;
    }

    public async Task<Dictionary<string, string>> LoadAsync(
        ResourceDefinition resource,
       CultureInfo culture,
       bool includeParents)
    {
        var cultureName = string.IsNullOrEmpty(culture.Name)
            ? _options.DefaultLanguage.Culture
            : culture.Name;

        var resources = ResolveResourceHierarchy(resource);

        var cacheKey = $"{resource.Name}-{cultureName}-{includeParents}";

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async (cancellationToken) =>
             await LoadFromStoresAsync(resource, resources, culture, includeParents)
            ,
            new RamshaCacheEntryOptions
            {
                Expiration = _options.ResourcesCacheExpiration,
                LocalCacheExpiration = _options.ResourcesLocalCacheExpiration
            },
            [
            LocalizationResourcesCacheTag,
             ResourceTagPrefix+resource.Name,
             CultureTagPrefix+cultureName,
             ..resources.Select(x=> ResourceTagPrefix+x.Name)
             ]
        );
    }

    public async Task<Dictionary<string, string>> LoadFromStoresAsync(
        ResourceDefinition resource,
        IReadOnlyList<ResourceDefinition> resources,
        CultureInfo culture,
        bool includeParents)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        await using var scope = _scopeFactory.CreateAsyncScope();
        var stores = GetStores(scope.ServiceProvider);
        var currentCulture = culture;

        while (currentCulture != CultureInfo.InvariantCulture)
        {
            foreach (var store in stores)
            {
                var filteredResources = resources.Where(res => !ShouldSkipStore(store.GetName(), res.Stores.ToList())).ToList();
                if (filteredResources.Count == 0)
                    continue;

                await store.FillAsync(result, resource, filteredResources, currentCulture.Name);
            }

            if (!includeParents)
                break;

            currentCulture = currentCulture.Parent;
        }

        return result;
    }

    public IReadOnlyList<ResourceDefinition> ResolveResourceHierarchy(
        ResourceDefinition root)
    {
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ordered = new List<ResourceDefinition>();
        var stack = new Stack<ResourceDefinition>();

        stack.Push(root);

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            if (!visited.Add(current.Name))
            {
                continue;
            }

            ordered.Add(current);

            if (current.Extends.Count > 0)
            {
                for (int i = current.Extends.Count - 1; i >= 0; i--)
                {
                    var name = current.Extends[i];

                    if (_options.Resources.TryGetValue(name, out var extended))
                    {
                        stack.Push(extended);
                    }
                }
            }
        }

        ordered.Reverse();
        return ordered;
    }


    private bool ShouldSkipStore(string store, List<string> allowedStores)
    => allowedStores.Any() &&
            !allowedStores.Contains(store);


    private List<ILocalizationResourceStore> GetStores(IServiceProvider serviceProvider)
    {
        var stores = _options
            .ResourcesStores
            .Select(type => (ILocalizationResourceStore)serviceProvider.GetRequiredService(type))
            .ToList();

        var multipleStores = stores.GroupBy(p => p.GetName()).FirstOrDefault(x => x.Count() > 1);
        if (multipleStores != null)
        {
            throw new Exception($"Duplicate LocalizationResource stores name detected");
        }

        return stores;
    }
}
