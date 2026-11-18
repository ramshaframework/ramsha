using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ramsha.Common.Domain;

namespace Ramsha.EntityFrameworkCore;

public static class RamshaDbContextOptionsFactory
{
    public static DbContextOptions<TDbContext> Create<TDbContext>(IServiceProvider serviceProvider)
        where TDbContext : RamshaEFDbContext<TDbContext>
    {
        var creationContext = GetCreationContext<TDbContext>(serviceProvider);

        var context = new RamshaDbContextConfigurationContext<TDbContext>(
            creationContext.ConnectionString,
            serviceProvider,
            creationContext.ConnectionStringName,
            creationContext.ExistingConnection
        );

        var options = GetDbContextOptions<TDbContext>(serviceProvider);

        PreConfigure(options, context);
        Configure(options, context);

        context.DbContextOptions.AddAbpDbContextOptionsExtension<TDbContext>();

        return context.DbContextOptions.Options;
    }

    private static void PreConfigure<TDbContext>(
        RamshaDbContextOptions options,
        RamshaDbContextConfigurationContext<TDbContext> context)
        where TDbContext : RamshaEFDbContext<TDbContext>
    {
        foreach (var defaultPreConfigureAction in options.DefaultPreConfigureActions)
        {
            defaultPreConfigureAction.Invoke(context);
        }

        var preConfigureActions = options.PreConfigureActions.FirstOrDefault(x => x.Key == typeof(TDbContext)).Value;
        if (preConfigureActions is not null && preConfigureActions.Count > 0)
        {
            foreach (var preConfigureAction in preConfigureActions!)
            {
                ((Action<RamshaDbContextConfigurationContext<TDbContext>>)preConfigureAction).Invoke(context);
            }
        }
    }

    private static void Configure<TDbContext>(
        RamshaDbContextOptions options,
        RamshaDbContextConfigurationContext<TDbContext> context)
        where TDbContext : RamshaEFDbContext<TDbContext>
    {
        var configureAction = options.ConfigureActions.FirstOrDefault(x => x.Key == typeof(TDbContext)).Value;
        if (configureAction != null)
        {
            ((Action<RamshaDbContextConfigurationContext<TDbContext>>)configureAction).Invoke(context);
        }
        else if (options.DefaultConfigureAction != null)
        {
            options.DefaultConfigureAction.Invoke(context);
        }
        else
        {
            throw new Exception(
                $"No configuration found for {typeof(DbContext).AssemblyQualifiedName}! Use services.Configure<AbpDbContextOptions>(...) to configure it.");
        }
    }

    private static RamshaDbContextOptions GetDbContextOptions<TDbContext>(IServiceProvider serviceProvider)
        where TDbContext : RamshaEFDbContext<TDbContext>
    {
        return serviceProvider.GetRequiredService<IOptions<RamshaDbContextOptions>>().Value;
    }

    private static DbContextCreationContext GetCreationContext<TDbContext>(IServiceProvider serviceProvider)
        where TDbContext : RamshaEFDbContext<TDbContext>
    {
        var context = DbContextCreationContext.Current;
        if (context != null)
        {
            return context;
        }

        var connectionStringName = ConnectionStringAttribute.GetNameOrNull<TDbContext>();
        var connectionString = serviceProvider
        .GetRequiredService<IConnectionStringResolver>()
        .Resolve(connectionStringName);


        return new DbContextCreationContext(
            connectionStringName,
            connectionString
        );
    }
}
