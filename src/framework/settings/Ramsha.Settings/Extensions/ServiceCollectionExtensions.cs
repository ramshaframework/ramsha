
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ramsha.Settings;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSettingsServices(this IServiceCollection services)
    {

        services.AddSingleton<InMemorySettingDefinitionStore>();
        services.AddTransient<ISettingDefinitionStore>(p => p.GetRequiredService<InMemorySettingDefinitionStore>());


        services.AddSingleton<NoneSettingStore>();
        services.TryAddTransient<ISettingStore>(p => p.GetRequiredService<NoneSettingStore>());

        services.AddTransient<ISettingResolver, SettingResolver>();


        services.AddScoped<ISettingValueResolverManager, SettingValueResolverManager>();


        var settingValueResolverInterfaceType = typeof(ISettingValueResolver);

        var permissionResolvers = RamshaTypeHelpers.GetImplementationTypes<SettingsModule>(settingValueResolverInterfaceType);
        foreach (var resolver in permissionResolvers)
        {
            services.AddTransient(settingValueResolverInterfaceType, resolver);
            services.AddTransient(resolver);
        }


        var definitionProviderInterfaceType = typeof(ISettingDefinitionProvider);
        var definitionProviderTypes = RamshaTypeHelpers.GetImplementationTypes<SettingsModule>(definitionProviderInterfaceType);
        foreach (var provider in definitionProviderTypes)
        {
            services.AddTransient(provider);

            services.AddTransient(definitionProviderInterfaceType, provider);
        }


        services.Configure<RamshaSettingsOptions>(options =>
       {
           options.ValueResolvers.Add<ConfigurationSettingValueResolver>();
           options.ValueResolvers.Add<MemorySettingValueResolver>();
       });


        return services;
    }
}
