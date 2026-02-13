using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ramsha.Caching;

namespace Ramsha.Localization;

public sealed class JsonLocalizationResourcesStore
    : LocalizationResourceStore, IDisposable
{
    public string Name => "j";
    private readonly Dictionary<string, FileSystemWatcher> _watchers = new();
    private readonly Dictionary<string, DateTime> _lastChanges = new();
    private readonly object _debounceLock = new();
    private static readonly TimeSpan _debounceWindow = TimeSpan.FromMilliseconds(500);

    private readonly RamshaLocalizationOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger _logger;
    private readonly IRamshaCache _cache;
    public JsonLocalizationResourcesStore(
        IOptions<RamshaLocalizationOptions> options,
        ILogger<JsonLocalizationResourcesStore> logger,
        IRamshaCache ramshaCache) : base("J")
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

    public override async Task FillAsync(
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

        SetupFileWatchers(resourceHierarchy, culture);

    }

    private void SetupFileWatchers(IReadOnlyList<ResourceDefinition> resourceHierarchy, string culture)
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

                watcher.Changed += (sender, e) => OnFileChanged(sender, e, resource);
                watcher.Created += (sender, e) => OnFileChanged(sender, e, resource);
                watcher.Deleted += (sender, e) => OnFileChanged(sender, e, resource);
                watcher.Renamed += (sender, e) => OnFileChanged(sender, e, resource);

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



    private void OnFileChanged(
     object? _,
     FileSystemEventArgs e,
     ResourceDefinition changedResource)
    {
        if (!e.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            return;

        lock (_debounceLock)
        {
            var now = DateTime.UtcNow;

            if (_lastChanges.TryGetValue(e.FullPath, out var lastChange))
            {
                if (now - lastChange < _debounceWindow)
                {
                    return;
                }
            }

            _lastChanges[e.FullPath] = now;
        }

        _ = HandleFileChangeAsync(e, changedResource);
    }

    private async Task HandleFileChangeAsync(
    FileSystemEventArgs e,
    ResourceDefinition changedResource)
    {
        try
        {
            await Task.Delay(200);

            _logger.LogInformation(
                "File changed: {File} , clearing cache , changed resource {ChangedResource}",
                e.Name,
                changedResource.Name);

            await _cache.RemoveByTagAsync(RamshaResourcesLoader.ResourceTagPrefix + changedResource.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error clearing cache after file change for resource {Resource}",
                changedResource.Name);
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

