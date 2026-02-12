
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Ramsha;
using Ramsha.Localization;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRamshaLocalizationServices(this IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton<IStringLocalizerFactory, RamshaStringLocalizerFactory>());
        services.AddTransient(typeof(IStringLocalizer<>), typeof(RamshaStringLocalizer<>));

        var resourcesStoreInterfaceType = typeof(ILocalizationResourceStore);

        var stores = RamshaTypeHelpers.GetImplementationTypes<LocalizationModule>(resourcesStoreInterfaceType);
        foreach (var store in stores)
        {
            services.AddTransient(resourcesStoreInterfaceType, store);
            services.AddTransient(store);
        }

        services.Configure<RamshaLocalizationOptions>(options =>
        {
            options.ResourcesStores.Add<JsonLocalizationResourcesStore>();
        });

        services.AddSingleton<IRamshaResourcesLoader, RamshaResourcesLoader>();

        return services;
    }
}
