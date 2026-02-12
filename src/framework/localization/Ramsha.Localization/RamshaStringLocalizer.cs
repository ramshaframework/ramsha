using System.Globalization;
using Microsoft.Extensions.Localization;
using Ramsha.Localization.Abstractions;

namespace Ramsha.Localization;

public class RamshaStringLocalizer : IRamshaStringLocalizer
{
    private readonly IRamshaResourcesLoader _resourcesLoader;
    private readonly ResourceDefinition _resource;

    public RamshaStringLocalizer(
        ResourceDefinition resource,
        IRamshaResourcesLoader resourcesLoader)
    {
        _resource = resource;
        _resourcesLoader = resourcesLoader;
    }


    public LocalizedString this[string name]
    {
        get
        {
            var value = GetStringAsync(name).GetAwaiter().GetResult();
            return new LocalizedString(name, value ?? name, value is null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
        => new(name, string.Format(this[name].Value, arguments), false);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var data = _resourcesLoader.LoadAsync(_resource, CultureInfo.CurrentUICulture, includeParentCultures).GetAwaiter().GetResult();
        return data.Select(x => new LocalizedString(x.Key, x.Value, false));
    }

    private async Task<string?> GetStringAsync(string key)
    {
        var culture = CultureInfo.CurrentUICulture;

        while (culture != CultureInfo.InvariantCulture)
        {
            var resources = await _resourcesLoader.LoadAsync(_resource, culture, false);
            if (resources.TryGetValue(key, out var value))
                return value;

            culture = culture.Parent;
        }

        return null;
    }


}



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
