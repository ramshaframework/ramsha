using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ramsha;

public static class RamshaFactory
{
    public static IInternalRamshaAppEngine CreateApp<TStartupModule>(
      Action<AppCreationOptions>? optionsAction = null)
      where TStartupModule : IRamshaModule
    {
        return CreateApp(typeof(TStartupModule), optionsAction);
    }

    public static IInternalRamshaAppEngine CreateApp(
        [NotNull] Type startupModuleType,
        Action<AppCreationOptions>? optionsAction = null)
    {
        var app = new InternalRamshaAppEngine(startupModuleType, optionsAction);
        Task.Run(() => app.ConfigureAsync()).GetAwaiter().GetResult();
        return app;
    }
    public async static Task<IInternalRamshaAppEngine> CreateAppAsync<TStartupModule>(
        Action<AppCreationOptions>? optionsAction = null)
        where TStartupModule : IRamshaModule
    {
        var app = CreateApp(typeof(TStartupModule), optionsAction);
        await app.ConfigureAsync();
        return app;
    }

    public async static Task<IInternalRamshaAppEngine> CreateAppAsync(
        [NotNull] Type startupModuleType,
        Action<AppCreationOptions>? optionsAction = null)
    {
        var app = new InternalRamshaAppEngine(startupModuleType, optionsAction);
        await app.ConfigureAsync();
        return app;
    }

    public async static Task<IExternalRamshaAppEngine> CreateAppAsync<TStartupModule>(
        [NotNull] IServiceCollection services,
        Action<AppCreationOptions>? optionsAction = null)
        where TStartupModule : IRamshaModule
    {
        return await CreateAppAsync(typeof(TStartupModule), services, optionsAction);
    }

    public async static Task<IExternalRamshaAppEngine> CreateAppAsync(
        [NotNull] Type startupModuleType,
        [NotNull] IServiceCollection services,
        Action<AppCreationOptions>? optionsAction = null)
    {
        var app = new ExternalRamshaAppEngine(startupModuleType, services, optionsAction);
        await app.ConfigureAsync();
        return app;
    }



    public static IExternalRamshaAppEngine CreateApp<TStartupModule>(
        [NotNull] IServiceCollection services,
        Action<AppCreationOptions>? optionsAction = null)
        where TStartupModule : IRamshaModule
    {
        return CreateApp(typeof(TStartupModule), services, optionsAction);
    }

    public static IExternalRamshaAppEngine CreateApp(
    Type startupModuleType,
    [NotNull] IServiceCollection services,
    Action<AppCreationOptions>? optionsAction = null)
    {
        var app = new ExternalRamshaAppEngine(startupModuleType, services, options =>
        {
            optionsAction?.Invoke(options);
        });
        Task.Run(() => app.ConfigureAsync()).GetAwaiter().GetResult();
        return app;
    }



}
