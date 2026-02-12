using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using Ramsha.Localization;

namespace Ramsha.AspNetCore;

public class RamshaRequestLocalizationOptionsProvider :
    IRamshaRequestLocalizationOptionsProvider

{
    private readonly IServiceScopeFactory _serviceProviderFactory;
    private readonly SemaphoreSlim _syncSemaphore;
    private Action<RequestLocalizationOptions>? _optionsAction;
    private RequestLocalizationOptions? _requestLocalizationOptions;

    public RamshaRequestLocalizationOptionsProvider(IServiceScopeFactory serviceProviderFactory)
    {
        _serviceProviderFactory = serviceProviderFactory;
        _syncSemaphore = new SemaphoreSlim(1, 1);
    }

    public virtual void InitializeOptions(Action<RequestLocalizationOptions>? optionsAction = null)
    {
        _optionsAction = optionsAction;
    }

    public virtual async Task<RequestLocalizationOptions> GetOptionsAsync()
    {
        if (_requestLocalizationOptions == null)
        {
            using (await _syncSemaphore.LockAsync())
            {
                if (_requestLocalizationOptions == null)
                {
                    using (var serviceScope = _serviceProviderFactory.CreateScope())
                    {
                        var localizationsOptions = serviceScope.ServiceProvider.GetRequiredService<IOptions<RamshaLocalizationOptions>>().Value;
                        var languagesProvider = serviceScope.ServiceProvider.GetRequiredService<ILocalizationLanguagesProvider>();

                        var languages = await languagesProvider.GetSupportedLanguagesAsync();

                        var options = !languages.Any()
                            ? new RequestLocalizationOptions()
                            : new RequestLocalizationOptions
                            {
                                SupportedCultures = languages
                                    .Select(l => l.Culture)
                                    .Distinct()
                                    .Select(c => new CultureInfo(c))
                                    .ToArray(),
                                SupportedUICultures = languages
                                    .Select(l => l.UiCulture)
                                    .Distinct()
                                    .Select(c => new CultureInfo(c))
                                    .ToArray()
                            };

                        foreach (var configurator in serviceScope.ServiceProvider
                                     .GetRequiredService<IOptions<RamshaRequestLocalizationOptions>>()
                                     .Value.RequestLocalizationOptionConfigurators)
                        {
                            await configurator(serviceScope.ServiceProvider, options);
                        }

                        _optionsAction?.Invoke(options);
                        _requestLocalizationOptions = options;
                    }
                }
            }
        }

        return _requestLocalizationOptions;
    }


}
