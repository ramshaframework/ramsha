using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Ramsha.Localization.Abstractions;

namespace Ramsha.Localization;


public class RamshaStringLocalizer : IRamshaStringLocalizer
{
    private readonly ConcurrentDictionary<string, Lazy<ConcurrentDictionary<string, string>>> _cache = new();
    private readonly RamshaLocalizationOptions _options;
    private readonly ResourceDefinition _resource;
    private readonly IReadOnlyList<ILocalizationResourceProvider> _providers;
    private readonly ILogger<RamshaStringLocalizer> _logger;

    public RamshaStringLocalizer(
        string resourceName,
        RamshaLocalizationOptions options,
        IEnumerable<ILocalizationResourceProvider> providers,
        ILogger<RamshaStringLocalizer>? logger = null)
    {
        _options = options;
        _resource = options.Resources.Get(resourceName);
        _providers = providers.ToList();
        _logger = logger ?? NullLogger<RamshaStringLocalizer>.Instance;

        foreach (var provider in _providers.OfType<ILocalizationChangeNotifier>())
        {
            provider.OnChangeAsync(_resource, () =>
            {
                ClearCache();
                return Task.CompletedTask;
            });
        }
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, value is null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
        => new(name, string.Format(this[name].Value, arguments), false);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var data = LoadAllResources(CultureInfo.CurrentUICulture, includeParentCultures);
        return data.Select(x => new LocalizedString(x.Key, x.Value, false));
    }

    private string? GetString(string key)
    {
        var culture = CultureInfo.CurrentUICulture;

        while (culture != CultureInfo.InvariantCulture)
        {
            var resources = LoadAllResources(culture, false);
            if (resources.TryGetValue(key, out var value))
                return value;

            culture = culture.Parent;
        }

        return null;
    }


    private ConcurrentDictionary<string, string> LoadAllResources(
        CultureInfo culture,
        bool includeParents)
    {
        var cultureName = string.IsNullOrEmpty(culture.Name)
            ? _options.DefaultLanguage.Culture
            : culture.Name;

        var cacheKey = $"{_resource.Name}_{cultureName}_{includeParents}";

        return _cache.GetOrAdd(cacheKey, _ =>
            new Lazy<ConcurrentDictionary<string, string>>(
                () => BuildResources(culture, includeParents),
                LazyThreadSafetyMode.ExecutionAndPublication))
            .Value;
    }

    private ConcurrentDictionary<string, string> BuildResources(
        CultureInfo culture,
        bool includeParents)
    {
        var result = new ConcurrentDictionary<string, string>(
            StringComparer.OrdinalIgnoreCase);

        var resources = _options.Resources.ResolveResourceHierarchy(_resource);

        foreach (var res in resources)
        {
            var currentCulture = culture;

            while (currentCulture != CultureInfo.InvariantCulture)
            {
                foreach (var provider in _providers)
                {
                    var data = provider.LoadAsync(res, currentCulture.Name).GetAwaiter().GetResult();
                    foreach (var kv in data)
                        result[kv.Key] = kv.Value;


                }

                if (!includeParents)
                    break;

                currentCulture = currentCulture.Parent;
            }
        }

        return result;
    }

    public void ClearCache(string? cacheKey = null)
    {
        if (cacheKey != null)
        {
            _cache.TryRemove(cacheKey, out _);
            _logger.LogInformation("cache cleared for {CacheKey}", cacheKey);
        }
        else
        {
            _cache.Clear();
            _logger.LogInformation("localization cache cleared");
        }
    }


}











// public class RamshaStringLocalizer : IRamshaStringLocalizer
// {
//     private readonly ConcurrentDictionary<string, Lazy<ConcurrentDictionary<string, string>>> _resourcesCache = new();
//     private readonly JsonSerializerOptions _jsonSerializerOptions;
//     private readonly RamshaLocalizationOptions _localizationOptions;
//     private readonly ResourceDefinition _resource;
//     private readonly ILogger<RamshaStringLocalizer> _logger;
//     private readonly IReadOnlyList<ILocalizationResourceProvider> _providers;

//     public RamshaStringLocalizer(
//         string resourceName,
//         RamshaLocalizationOptions localizationOptions,
//         IEnumerable<ILocalizationResourceProvider> providers,
//         ILogger<RamshaStringLocalizer>? logger = null)
//     {
//         _resource = localizationOptions.Resources.Get(resourceName) ??
//             throw new ArgumentException($"Resource '{resourceName}' not found in localization options");

//         _localizationOptions = localizationOptions;
//         _logger = logger ?? NullLogger<RamshaStringLocalizer>.Instance;

//         _jsonSerializerOptions = new JsonSerializerOptions
//         {
//             PropertyNameCaseInsensitive = true,
//             ReadCommentHandling = JsonCommentHandling.Skip,
//             AllowTrailingCommas = true,
//             Converters = { new JsonStringEnumConverter() }
//         };
//         _providers = providers.ToList();
//     }

//     public LocalizedString this[string name]
//     {
//         get
//         {
//             if (name is null)
//                 throw new ArgumentNullException(nameof(name));

//             var value = GetString(name);
//             var resourceNotFound = string.IsNullOrEmpty(value);
//             return new LocalizedString(name, value ?? name, resourceNotFound);
//         }
//     }

//     public LocalizedString this[string name, params object[] arguments]
//     {
//         get
//         {
//             if (name == null)
//                 throw new ArgumentNullException(nameof(name));

//             var actualValue = this[name];
//             return !actualValue.ResourceNotFound
//                 ? new LocalizedString(name, string.Format(actualValue.Value, arguments), false)
//                 : actualValue;
//         }
//     }

//     public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
//     {
//         var culture = CultureInfo.CurrentUICulture;
//         var resources = GetResourcesForCulture(culture, includeParentCultures);

//         return resources.Select(kvp => new LocalizedString(kvp.Key, kvp.Value, false));
//     }

//     private string? GetString(string key)
//     {
//         if (key == null)
//             throw new ArgumentNullException(nameof(key));

//         var culture = CultureInfo.CurrentUICulture;
//         var currentCulture = culture;

//         while (currentCulture != CultureInfo.InvariantCulture)
//         {
//             var resources = LoadAllResources(currentCulture);
//             if (resources.TryGetValue(key, out var value))
//             {
//                 return value;
//             }

//             currentCulture = currentCulture.Parent;
//         }

//         var defaultCulture = new CultureInfo(_localizationOptions.DefaultLanguage.Culture);
//         if (culture != defaultCulture)
//         {
//             var defaultResources = LoadAllResources(defaultCulture);
//             if (defaultResources.TryGetValue(key, out var defaultValue))
//             {
//                 return defaultValue;
//             }
//         }

//         return null;
//     }

//     private ConcurrentDictionary<string, string> LoadAllResources(CultureInfo culture)
//     {
//         var cultureName = string.IsNullOrEmpty(culture.Name)
//             ? _localizationOptions.DefaultLanguage.Culture
//             : culture.Name;

//         var cacheKey = $"{_resource.Name}_{cultureName}";

//         return _resourcesCache.GetOrAdd(cacheKey, key =>
//             new Lazy<ConcurrentDictionary<string, string>>(() =>
//                 BuildResourcesForCulture(cultureName),
//                 LazyThreadSafetyMode.ExecutionAndPublication))
//             .Value;
//     }

//     private ConcurrentDictionary<string, string> BuildResourcesForCulture(string cultureName)
//     {
//         var allResources = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
//         var visitedResources = new HashSet<string>();

//         LoadResourceHierarchyIterative(_resource, cultureName, allResources, visitedResources);

//         return allResources;
//     }

//     private void LoadResourceHierarchyIterative(
//         ResourceDefinition startResource,
//         string cultureName,
//         ConcurrentDictionary<string, string> allResources,
//         HashSet<string> visitedResources)
//     {
//         var stack = new Stack<ResourceDefinition>();
//         stack.Push(startResource);

//         var processingOrder = new List<ResourceDefinition>();

//         while (stack.Count > 0)
//         {
//             var current = stack.Pop();

//             if (visitedResources.Contains(current.Name))
//             {
//                 _logger?.LogWarning("Circular dependency detected for resource: {ResourceName}", current.Name);
//                 continue;
//             }

//             visitedResources.Add(current.Name);
//             processingOrder.Add(current);

//             if (current.Extends != null && current.Extends.Count > 0)
//             {
//                 for (int i = current.Extends.Count - 1; i >= 0; i--)
//                 {
//                     var extendedResourceName = current.Extends[i];
//                     var extendedResource = _localizationOptions.Resources.Get(extendedResourceName);
//                     if (extendedResource != null)
//                     {
//                         stack.Push(extendedResource);
//                     }
//                     else
//                     {
//                         _logger?.LogWarning("Extended resource '{ExtendedResourceName}' not found for resource '{ResourceName}'",
//                             extendedResourceName, current.Name);
//                     }
//                 }
//             }
//         }

//         processingOrder.Reverse();

//         foreach (var resource in processingOrder)
//         {
//             var resources = LoadResourceFiles(resource, cultureName);
//             foreach (var kvp in resources)
//             {
//                 allResources[kvp.Key] = kvp.Value;
//             }
//         }
//     }

//     private Dictionary<string, string> LoadResourceFiles(ResourceDefinition resource, string cultureName)
//     {
//         var resources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
//         var resourcePath = resource.Path ?? resource.Name.Split('.').Last();

//         var fileNames = new List<string>();

//         if (!string.IsNullOrEmpty(cultureName))
//         {
//             fileNames.Add($"{cultureName}.json");

//             var dashIndex = cultureName.IndexOf('-');
//             if (dashIndex > 0)
//             {
//                 fileNames.Add($"{cultureName.Substring(0, dashIndex)}.json");
//             }
//         }

//         var defaultCultureFileName = $"{_localizationOptions.DefaultLanguage.Culture}.json";
//         if (!fileNames.Contains(defaultCultureFileName))
//         {
//             fileNames.Add(defaultCultureFileName);
//         }

//         foreach (var fileName in fileNames.Distinct())
//         {
//             var filePath = Path.Combine(_localizationOptions.ResourcesPath, resourcePath, fileName);
//             if (File.Exists(filePath))
//             {
//                 try
//                 {
//                     var json = File.ReadAllText(filePath);
//                     var loadedResources = JsonSerializer.Deserialize<Dictionary<string, string>>(
//                         json, _jsonSerializerOptions);

//                     if (loadedResources != null)
//                     {
//                         foreach (var kvp in loadedResources)
//                         {
//                             if (!string.IsNullOrEmpty(kvp.Value))
//                             {
//                                 resources[kvp.Key] = kvp.Value;
//                             }
//                         }

//                         _logger?.LogDebug("Loaded {Count} resources from {FilePath}", loadedResources.Count, filePath);
//                     }
//                 }
//                 catch (JsonException ex)
//                 {
//                     _logger?.LogError(ex, "Error parsing JSON resource file {FilePath}: {ErrorMessage}",
//                         filePath, ex.Message);
//                 }
//                 catch (Exception ex)
//                 {
//                     _logger?.LogError(ex, "Error loading resource file {FilePath}: {ErrorMessage}",
//                         filePath, ex.Message);
//                 }
//             }
//             else
//             {
//                 _logger?.LogDebug("Resource file not found: {FilePath}", filePath);
//             }
//         }

//         return resources;
//     }

//     private Dictionary<string, string> GetResourcesForCulture(CultureInfo culture, bool includeParentCultures)
//     {
//         var allResources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
//         var currentCulture = culture;

//         while (currentCulture != CultureInfo.InvariantCulture)
//         {
//             var resources = LoadAllResources(currentCulture);
//             foreach (var kvp in resources)
//             {
//                 if (!allResources.ContainsKey(kvp.Key))
//                 {
//                     allResources[kvp.Key] = kvp.Value;
//                 }
//             }

//             if (!includeParentCultures)
//                 break;

//             currentCulture = currentCulture.Parent;
//         }

//         return allResources;
//     }

//     public void ClearCache()
//     {
//         _resourcesCache.Clear();
//         _logger?.LogInformation("Localization cache cleared");
//     }
// }


public class RamshaStringLocalizer<T> : IStringLocalizer<T>
{
    private readonly IStringLocalizer _localizer;

    public RamshaStringLocalizer(IStringLocalizerFactory factory)
    {
        _localizer = factory.Create(typeof(T));
    }

    public LocalizedString this[string name] => _localizer[name];

    public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        => _localizer.GetAllStrings(includeParentCultures);
}
