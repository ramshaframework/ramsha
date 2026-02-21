using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ramsha;

public abstract class RamshaEngine : IRamshaEngine
{
    public IReadOnlyList<IModuleDescriptor> Modules { get; private set; } = null!;
    protected bool _configured;
    protected readonly IServiceCollection _services;
    public IServiceProvider ServiceProvider { get; private set; } = default!;

    internal RamshaEngine(IServiceCollection services)
    {
        _services = services;
        services.AddSingleton<IRamshaEngine>(this);
        services.AddSingleton<IModuleContainer>(this);
        ConfigureCoreServices(_services);
    }


    protected virtual void ConfigureCoreServices(IServiceCollection services)
    {
        services.AddRamshaCoreServices();
        services.AddObjectAccessor<IServiceProvider>();
    }

    protected virtual void InternalSetServiceProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        ServiceProvider.GetRequiredService<ObjectAccessor<IServiceProvider>>().Value = ServiceProvider;
    }

    protected virtual async Task InitializeAsync()
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            ReplayLogs(scope.ServiceProvider);
            await scope.ServiceProvider
                .GetRequiredService<IRamshaHooksManager>()
                .Initialize(new InitContext(scope.ServiceProvider));
        }
    }

    public virtual async Task ShutdownAsync()
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            await scope.ServiceProvider
                .GetRequiredService<IRamshaHooksManager>()
                .Shutdown(new ShutdownContext(scope.ServiceProvider));
        }
    }

    protected virtual async Task ConfigureModulesService(BuildServicesContext context, IReadOnlyList<IModuleDescriptor> modules)
    {
        foreach (var module in modules)
        {
            try
            {
                await module.Instance.BuildServicesAsync(context);
            }

            catch (Exception ex)
            {
                throw new Exception($"An error occurred during {nameof(IRamshaModule.BuildServicesAsync)} phase of the module {module.Type.AssemblyQualifiedName}. See the inner exception for details.", ex);
            }
        }
    }

    protected virtual async Task PrepareModules(PrepareContext context, IReadOnlyList<IModuleDescriptor> modules)
    {
        foreach (var module in modules)
        {
            try
            {
                await module.Instance.PrepareAsync(context);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during {nameof(IRamshaModule.Prepare)} phase of the module {module.Type.AssemblyQualifiedName}. See the inner exception for details.", ex);
            }
        }
    }


    public virtual async Task ConfigureAsync()
    {
        if (_configured)
        {
            throw new RamshaException("Ramsha has already configured before");
        }


        Modules = LoadModules(_services);
        RamshaAssemblyHelpers.LoadSolutionDlls();


        var prepareContext = new PrepareContext(_services);
        _services.AddSingleton(prepareContext);

        await PrepareModules(prepareContext, Modules);

        var context = new BuildServicesContext(_services);
        _services.AddSingleton(context);

        await ConfigureModulesService(context, Modules);

        _services.AddRamshaHooks();
        _configured = true;
    }


    protected abstract IReadOnlyList<IModuleDescriptor> LoadModules(IServiceCollection services);

    protected virtual void ReplayLogs(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetService<ILogger<RamshaEngine>>();
        if (logger == null)
        {
            return;
        }

        var bootstrapLogger = serviceProvider.GetRequiredService<IBootstrapLoggerFactory>().Create<RamshaEngine>();
        bootstrapLogger.ReplayTo(logger);
        bootstrapLogger.ClearEntries();
    }



}
