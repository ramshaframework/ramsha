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
    private readonly IServiceProvider _serviceProvider;
    private readonly IRamshaCache _cache;
    private readonly RamshaLocalizationOptions _options;
    private readonly Lazy<List<ILocalizationResourceStore>> _lazyStores;
    public RamshaResourcesLoader(
        IOptions<RamshaLocalizationOptions> options,
        IRamshaCache cache,
         IServiceProvider serviceProvider)
    {
        _options = options.Value;
        _cache = cache;
        _serviceProvider = serviceProvider;
        _lazyStores = new(GetStores, true);
    }

    public async Task<Dictionary<string, string>> LoadAsync(
        ResourceDefinition resource,
       CultureInfo culture,
       bool includeParents)
    {
        var cultureName = string.IsNullOrEmpty(culture.Name)
            ? _options.DefaultLanguage.Culture
            : culture.Name;

        var cacheKey = $"{resource.Name}-{cultureName}";

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async (cancellationToken) => await LoadFromStoresAsync(resource, culture, includeParents),
            new RamshaCacheEntryOptions
            {
                Expiration = _options.ResourcesCacheExpiration,
                LocalCacheExpiration = _options.ResourcesLocalCacheExpiration
            },
            [
            LocalizationResourcesCacheTag,
             $"{ResourceTagPrefix}{resource.Name}",
             $"{CultureTagPrefix}{cultureName}"]
        );
    }

    public async Task<Dictionary<string, string>> LoadFromStoresAsync(
        ResourceDefinition resource,
        CultureInfo culture,
        bool includeParents)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var resources = ResolveResourceHierarchy(resource);
        var stores = _lazyStores.Value;
        var currentCulture = culture;

        while (currentCulture != CultureInfo.InvariantCulture)
        {
            foreach (var store in stores)
            {
                var filteredResources = resources.Where(res => !ShouldSkipStore(store.Name, res.Stores.ToList())).ToList();

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


    private List<ILocalizationResourceStore> GetStores()
    {
        var stores = _options
            .ResourcesStores
            .Select(type => (_serviceProvider.GetRequiredService(type) as ILocalizationResourceStore)!)
            .ToList();

        var multipleStores = stores.GroupBy(p => p.Name).FirstOrDefault(x => x.Count() > 1);
        if (multipleStores != null)
        {
            throw new Exception($"Duplicate LocalizationResource stores name detected");
        }

        return stores;
    }
}
