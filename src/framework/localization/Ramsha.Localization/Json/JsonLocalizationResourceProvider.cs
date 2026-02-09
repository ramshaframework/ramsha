using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ramsha.Localization.Json;

namespace Ramsha.Localization;

public sealed class JsonLocalizationResourceProvider
    : ILocalizationResourceProvider, ILocalizationChangeNotifier, IDisposable
{
    public string Name => "json";
    private readonly Dictionary<string, JsonFileChangeMonitor> _monitors = new();

    private readonly RamshaLocalizationOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger _logger;

    public JsonLocalizationResourceProvider(
        IOptions<RamshaLocalizationOptions> options,
        ILogger<JsonLocalizationResourceProvider> logger)
    {
        _options = options.Value;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
    }

    public async Task<IDictionary<string, string>> LoadAsync(
        ResourceDefinition resource,
        string culture,
        CancellationToken ct = default)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var resourcePath = resource.GetPathOrDefault(_options.ResourcesPath);

        var files = ResolveFileNames(culture);

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



        return result;
    }

    private static IEnumerable<string> ResolveFileNames(string culture)
    {
        yield return $"{culture}.json";

        var dash = culture.IndexOf('-');
        if (dash > 0)
            yield return $"{culture[..dash]}.json";
    }


    public Task OnChangeAsync(ResourceDefinition resource, Func<Task> onChange)
    {
        var resourcePath = resource.GetPathOrDefault(_options.ResourcesPath);

        if (!_monitors.ContainsKey(resourcePath))
        {
            var monitor = new JsonFileChangeMonitor(resourcePath, onChange);
            _monitors[resourcePath] = monitor;
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        foreach (var monitor in _monitors.Values)
            monitor.Dispose();
    }
}

