using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ramsha.Caching;

namespace Ramsha.Localization;

public sealed class JsonLocalizationResourcesStore
    : ILocalizationResourceStore, IDisposable
{
    public string Name => "j";
    private readonly Dictionary<string, FileSystemWatcher> _watchers = new();
    private readonly RamshaLocalizationOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger _logger;
    private readonly IRamshaCache _cache;
    public JsonLocalizationResourcesStore(
        IOptions<RamshaLocalizationOptions> options,
        ILogger<JsonLocalizationResourcesStore> logger,
        IRamshaCache ramshaCache)
    {
        _cache = ramshaCache;
        _options = options.Value;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
    }

    public async Task FillAsync(
        Dictionary<string, string> result,
        ResourceDefinition rootResource,
        IReadOnlyList<ResourceDefinition> resourceHierarchy,
        string culture,
        CancellationToken ct = default)
    {
        var files = ResolveFileNames(culture).ToList();

        foreach (var resource in resourceHierarchy)
        {
            var resourcePath = resource.GetPathOrDefault(_options.ResourcesPath);

            foreach (var file in files)
            {
                var path = Path.Combine(resourcePath, file);
                if (!File.Exists(path))
                    continue;

                try
                {
                    var json = await File.ReadAllTextAsync(path, ct);
                    var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json, _jsonOptions);

                    if (data != null)
                    {
                        foreach (var kv in data)
                            result[kv.Key] = kv.Value;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load {Path}", path);
                }
            }
        }

        SetupFileWatchers(rootResource, resourceHierarchy, culture);

    }

    private void SetupFileWatchers(ResourceDefinition rootResource, IReadOnlyList<ResourceDefinition> resourceHierarchy, string culture)
    {
        foreach (var resource in resourceHierarchy)
        {
            var path = resource.GetPathOrDefault(_options.ResourcesPath);


            if (!_watchers.ContainsKey(path))
            {
                var watcher = new FileSystemWatcher(path)
                {
                    Filter = "*.json",
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                    EnableRaisingEvents = true
                };

                watcher.Changed += (sender, e) => OnFileChanged(sender, e, rootResource, resource, culture);
                watcher.Created += (sender, e) => OnFileChanged(sender, e, rootResource, resource, culture);
                watcher.Deleted += (sender, e) => OnFileChanged(sender, e, rootResource, resource, culture);
                watcher.Renamed += (sender, e) => OnFileChanged(sender, e, rootResource, resource, culture);

                _watchers[path] = watcher;
                _logger.LogInformation("Watching: {Path} for resource {Resource}", path, resource.Name);
            }
        }
    }


    private static IEnumerable<string> ResolveFileNames(string culture)
    {
        yield return $"{culture}.json";

        var dash = culture.IndexOf('-');
        if (dash > 0)
            yield return $"{culture[..dash]}.json";
    }



    private async void OnFileChanged(object _, FileSystemEventArgs e, ResourceDefinition rootResource, ResourceDefinition changedResource, string culture)
    {
        if (!e.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            return;

        try
        {
            _logger.LogInformation("File changed: {File} , clearing cache for root resource {RootResource}, changed resource {ChangedResource}",
             e.Name, rootResource.Name, changedResource.Name);

            var cacheKey = $"{rootResource.Name}-{culture}";
            await _cache.RemoveAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache after file change for resource {Resource}", changedResource.Name);
        }
    }


    public void Dispose()
    {
        foreach (var watcher in _watchers.Values)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }

        _watchers.Clear();
    }
}

