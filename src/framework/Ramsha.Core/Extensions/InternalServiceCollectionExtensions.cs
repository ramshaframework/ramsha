
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ramsha;

namespace Microsoft.Extensions.DependencyInjection;

internal static class InternalServiceCollectionExtensions
{
    private static void AddMicrosoftCoreServices(IServiceCollection services)
    {
        services.AddOptions();
        services.AddLogging();
        services.AddLocalization();
    }

    internal static void AddRamshaHooks(this IServiceCollection services)
    {
        var initHookInterfaceType = typeof(IInitHookContributor);
        var initHooks = RamshaTypeHelpers.GetRamshaTypes(initHookInterfaceType);
        foreach (var hook in initHooks)
        {
            services.AddTransient(initHookInterfaceType, hook);
            services.AddTransient(hook);
        }

        var shutdownHookInterfaceType = typeof(IShutdownHookContributor);
        var shutdownHooks = RamshaTypeHelpers.GetImplementationTypes<RamshaModule>(shutdownHookInterfaceType);
        foreach (var hook in shutdownHooks)
        {
            services.AddTransient(shutdownHookInterfaceType, hook);
            services.AddTransient(hook);
        }
    }
    internal static void AddRamshaCoreServices(this IServiceCollection services)
    {
        AddMicrosoftCoreServices(services);
        services.TryAddSingleton<IBootstrapLoggerFactory>(new DefaultBootstrapLoggerFactory());
        services.AddSingleton<IRamshaHooksManager, RamshaHooksManager>();

        services.PrepareOptions<RamshaTypeReplacementOptions>(options => { });
    }
}


