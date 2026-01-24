
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Ramsha;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{

    public static IExternalRamshaEngine AddRamsha(this IServiceCollection services, Action<RamshaBuilder> action)
    {
        var builder = new RamshaBuilder();
        action(builder);

        var engine = services.GetSingletonInstanceOrNull<RamshaAsServiceEngine>()
        ?? new RamshaAsServiceEngine(services, builder.Modules, builder.PrepareActions);

        Task.Run(() => engine.ConfigureAsync())
            .GetAwaiter().GetResult();

        return engine;
    }

    public static async Task<IExternalRamshaEngine> AddRamshaAsync(this IServiceCollection services, Action<RamshaBuilder> action)
    {
        var builder = new RamshaBuilder();
        action(builder);

        var engine = services.GetSingletonInstanceOrNull<RamshaAsServiceEngine>()
        ?? new RamshaAsServiceEngine(services, builder.Modules, builder.PrepareActions);

        await engine.ConfigureAsync();

        return engine;
    }


    public static IServiceProvider BuildRamshaServiceProvider(this IServiceCollection services, ServiceProviderOptions? options = null, IServiceProviderResolver? resolver = null)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        return new RamshaServiceProvider(services, options, resolver);
    }

    public static IExternalRamshaAppEngine AddRamshaApp<TStartupModule>(
     [NotNull] this IServiceCollection services,
     Action<AppCreationOptions>? optionsAction = null)
     where TStartupModule : IRamshaModule
    {
        return RamshaFactory.CreateApp<TStartupModule>(services, optionsAction);
    }

    public static IExternalRamshaAppEngine AddRamshaApp(
        [NotNull] this IServiceCollection services,
        [NotNull] Type startupModuleType,
        Action<AppCreationOptions>? optionsAction = null)
    {
        return RamshaFactory.CreateApp(startupModuleType, services, optionsAction);
    }

    public async static Task<IExternalRamshaAppEngine> AddRamshaAppAsync<TStartupModule>(
        [NotNull] this IServiceCollection services,
        Action<AppCreationOptions>? optionsAction = null)
        where TStartupModule : IRamshaModule
    {
        return await RamshaFactory.CreateAppAsync<TStartupModule>(services, optionsAction);
    }


    public async static Task<IExternalRamshaAppEngine> AddRamshaAppAsync(
      [NotNull] this IServiceCollection services,
      [NotNull] Type startupModuleType,
      Action<AppCreationOptions>? optionsAction = null)
    {
        return await RamshaFactory.CreateAppAsync(startupModuleType, services, optionsAction);
    }

    public static string? GetApplicationName(this IServiceCollection services)
    {
        return services.GetSingletonInstance<IAppInfoAccessor>().ApplicationName;
    }

    public static string GetApplicationInstanceId(this IServiceCollection services)
    {
        return services.GetSingletonInstance<IAppInfoAccessor>().InstanceId;
    }

    public static IRamshaHostEnvironment GetRamshaHostEnvironment(this IServiceCollection services)
    {
        return services.GetSingletonInstance<IRamshaHostEnvironment>();
    }

    public static Type? NormalizedImplementationType(this ServiceDescriptor descriptor)
    {
        return descriptor.IsKeyedService ? descriptor.KeyedImplementationType : descriptor.ImplementationType;
    }

    public static object? NormalizedImplementationInstance(this ServiceDescriptor descriptor)
    {
        return descriptor.IsKeyedService ? descriptor.KeyedImplementationInstance : descriptor.ImplementationInstance;
    }

    public static bool IsAdded<T>(this IServiceCollection services)
    {
        return services.IsAdded(typeof(T));
    }

    public static bool IsAdded(this IServiceCollection services, Type type)
    {
        return services.Any(d => d.ServiceType == type);
    }

    public static T? GetSingletonInstanceOrNull<T>(this IServiceCollection services)
    {
        return (T?)services
            .FirstOrDefault(d => d.ServiceType == typeof(T))
            ?.NormalizedImplementationInstance();
    }

    public static T GetSingletonInstance<T>(this IServiceCollection services)
    {
        var service = services.GetSingletonInstanceOrNull<T>();
        if (service == null)
        {
            throw new InvalidOperationException("Could not find singleton service: " + typeof(T).AssemblyQualifiedName);
        }

        return service;
    }

    public static IServiceProvider BuildServiceProviderFromFactory([NotNull] this IServiceCollection services)
    {

        foreach (var service in services)
        {
            var factoryInterface = service.NormalizedImplementationInstance()?.GetType()
                .GetTypeInfo()
                .GetInterfaces()
                .FirstOrDefault(i => i.GetTypeInfo().IsGenericType &&
                                     i.GetGenericTypeDefinition() == typeof(IServiceProviderFactory<>));

            if (factoryInterface == null)
            {
                continue;
            }

            var containerBuilderType = factoryInterface.GenericTypeArguments[0];
            return (IServiceProvider)typeof(ServiceCollectionExtensions)
                .GetTypeInfo()
                .GetMethods()
                .Single(m => m.Name == nameof(BuildServiceProviderFromFactory) && m.IsGenericMethod)
                .MakeGenericMethod(containerBuilderType)
                .Invoke(null, new object?[] { services, null })!;
        }

        return services.BuildServiceProvider();
    }

    public static void InjectProperties(this IServiceProvider provider, Type type, object service)
    {
        if (service is null) return;
        var propInfos = service.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.GetCustomAttribute<InjectableAttribute>() != null)
            .ToList();
        foreach (var propInfo in propInfos)
        {
            var instance = provider.GetService(propInfo.PropertyType);
            propInfo.SetValue(service, instance);
        }
    }





    public static IServiceProvider BuildServiceProviderFromFactory<TContainerBuilder>([NotNull] this IServiceCollection services, Action<TContainerBuilder>? builderAction = null) where TContainerBuilder : notnull
    {

        var serviceProviderFactory = services.GetSingletonInstanceOrNull<IServiceProviderFactory<TContainerBuilder>>();
        if (serviceProviderFactory == null)
        {
            throw new Exception($"Could not find {typeof(IServiceProviderFactory<TContainerBuilder>).FullName} in {services}.");
        }

        var builder = serviceProviderFactory.CreateBuilder(services);
        builderAction?.Invoke(builder);
        return serviceProviderFactory.CreateServiceProvider(builder);
    }


    internal static T? GetService<T>(this IServiceCollection services)
    {
        return services
            .GetSingletonInstance<IRamshaEngine>()
            .ServiceProvider
            .GetService<T>();
    }


    internal static object? GetService(this IServiceCollection services, Type type)
    {
        return services
            .GetSingletonInstance<IRamshaEngine>()
            .ServiceProvider
            .GetService(type);
    }


    public static T GetRequiredService<T>(this IServiceCollection services) where T : notnull
    {
        return services
            .GetSingletonInstance<IRamshaEngine>()
            .ServiceProvider
            .GetRequiredService<T>();
    }


    public static object GetRequiredService(this IServiceCollection services, Type type)
    {
        return services
            .GetSingletonInstance<IRamshaEngine>()
            .ServiceProvider
            .GetRequiredService(type);
    }

    public static Lazy<T?> GetServiceLazy<T>(this IServiceCollection services)
    {
        return new Lazy<T?>(services.GetService<T>, true);
    }

    public static Lazy<object?> GetServiceLazy(this IServiceCollection services, Type type)
    {
        return new Lazy<object?>(() => services.GetService(type), true);
    }


    public static Lazy<T> GetRequiredServiceLazy<T>(this IServiceCollection services) where T : notnull
    {
        return new Lazy<T>(services.GetRequiredService<T>, true);
    }


    public static Lazy<object> GetRequiredServiceLazy(this IServiceCollection services, Type type)
    {
        return new Lazy<object>(() => services.GetRequiredService(type), true);
    }

    public static IServiceProvider? GetServiceProviderOrNull(this IServiceCollection services)
    {
        return services.GetObjectOrNull<IServiceProvider>();
    }

    public static T? GetSingletonOrNull<T>(this IServiceCollection services)
    {
        return (T?)services.FirstOrDefault(d => d.ServiceType == typeof(T))
        ?.NormalizedImplementationInstance();
    }

    public static IServiceCollection ReplaceConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        return services.Replace(ServiceDescriptor.Singleton<IConfiguration>(configuration));
    }

    public static IConfiguration GetConfiguration(this IServiceCollection services)
    {
        return services.GetConfigurationOrNull() ??
               throw new Exception("Could not find an implementation of " + typeof(IConfiguration).AssemblyQualifiedName + " in the service collection.");
    }

    public static IConfiguration? GetConfigurationOrNull(this IServiceCollection services)
    {
        var hostBuilderContext = services.GetSingletonInstanceOrNull<HostBuilderContext>();
        if (hostBuilderContext?.Configuration != null)
        {
            return hostBuilderContext.Configuration as IConfigurationRoot;
        }

        return services.GetSingletonInstanceOrNull<IConfiguration>();
    }
}
