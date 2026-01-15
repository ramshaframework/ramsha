using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ramsha;
using Ramsha.Common.Domain;
using Ramsha.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRamshaDbContext(
    this IServiceCollection services,
    Type dbContextInterfaceType,
    Type dbContextType,
    Action<IDbContextRegistrationOptionsBaseBuilder>? optionsBuilder = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (dbContextInterfaceType == null)
            throw new ArgumentNullException(nameof(dbContextInterfaceType));

        if (dbContextType == null)
            throw new ArgumentNullException(nameof(dbContextType));

        if (!typeof(IRamshaEFDbContext).IsAssignableFrom(dbContextInterfaceType))
            throw new ArgumentException($"{dbContextInterfaceType.Name} must implement IRamshaEFDbContext.", nameof(dbContextInterfaceType));

        services.AddMemoryCache();

        services.TryAddTransient(dbContextType, sp => sp.CreateInstanceWithPropInjection(dbContextType));

        services.AddTransient(dbContextInterfaceType, sp => sp.GetRequiredService(dbContextType));
        services.AddTransient(typeof(IRamshaEFDbContext), sp => sp.GetRequiredService(dbContextType));
        services.AddTransient(typeof(IEFDbContext), sp => sp.GetRequiredService(dbContextType));

        var options = new EfDbContextRegistrationOptions(dbContextType, services);

        optionsBuilder?.Invoke(options);

        var createMethod = typeof(RamshaDbContextOptionsFactory)
          .GetMethod(nameof(RamshaDbContextOptionsFactory.Create))!
          .MakeGenericMethod(dbContextType);

        var dbContextOptionsType = typeof(DbContextOptions<>).MakeGenericType(dbContextType);

        services.TryAddTransient(dbContextOptionsType, sp =>
        {
            return createMethod.Invoke(null, new object?[] { sp });
        });

        services.AddTransient(dbContextInterfaceType, sp => sp.GetRequiredService(dbContextType));


        foreach (var entry in options.ReplacedDbContextTypes)
        {
            var originalDbContextType = entry.Key;
            var targetDbContextType = entry.Value ?? dbContextType;

            services.Replace(ServiceDescriptor.Transient(originalDbContextType, sp =>
            {
                var provider = sp.GetRequiredService<IEfDbContextTypeProvider>();
                var resolvedType = provider.GetDbContextType(originalDbContextType);
                return sp.GetRequiredService(resolvedType);
            }));

            services.Configure<RamshaDbContextOptions>(opts =>
            {
                opts.DbContextReplacements[originalDbContextType] = targetDbContextType;
            });
        }

        new EfRepositoryRegistrar(options).AddRepositories();
        new EFGlobalQueryFilterRegistrar(options).RegisterFilters();

        return services;
    }

    public static IServiceCollection AddRamshaDbContext<TDbContextInterface, TDbContext>(
        this IServiceCollection services,
           Action<IDbContextRegistrationOptionsBaseBuilder>? optionsBuilder = null)
           where TDbContextInterface : class, IRamshaEFDbContext
        where TDbContext : RamshaEFDbContext<TDbContext>, TDbContextInterface
    {
        services.AddMemoryCache();

        services.TryAddTransient(sp => sp.CreateInstanceWithPropInjection<TDbContext>());

        services.AddTransient<TDbContextInterface>(sp => sp.GetRequiredService<TDbContext>());
        services.AddTransient<IRamshaEFDbContext>(sp => sp.GetRequiredService<TDbContext>());
        services.AddTransient<IEFDbContext>(sp => sp.GetRequiredService<TDbContext>());


        var options = new EfDbContextRegistrationOptions(typeof(TDbContext), services);

        optionsBuilder?.Invoke(options);
        services.TryAddTransient(RamshaDbContextOptionsFactory.Create<TDbContext>);

        foreach (var entry in options.ReplacedDbContextTypes)
        {
            var originalDbContextType = entry.Key;
            var targetDbContextType = entry.Value ?? typeof(TDbContext);

            services.Replace(ServiceDescriptor.Transient(originalDbContextType, sp =>
            {
                var dbContextType = sp.GetRequiredService<IEfDbContextTypeProvider>().GetDbContextType(originalDbContextType);
                return sp.GetRequiredService(dbContextType);
            }));

            services.Configure<RamshaDbContextOptions>(opts =>
            {
                opts.DbContextReplacements[originalDbContextType] = targetDbContextType;
            });
        }

        new EfRepositoryRegistrar(options).AddRepositories();
        new EFGlobalQueryFilterRegistrar(options).RegisterFilters();

        return services;
    }
    public static IServiceCollection AddRamshaDbContext<TDbContext>(
        this IServiceCollection services,
           Action<IDbContextRegistrationOptionsBaseBuilder>? optionsBuilder = null)
        where TDbContext : RamshaEFDbContext<TDbContext>
    {
        services.AddMemoryCache();

        services.TryAddTransient(sp => sp.CreateInstanceWithPropInjection<TDbContext>());

        services.AddTransient<IRamshaEFDbContext>(sp => sp.GetRequiredService<TDbContext>());
        services.AddTransient<IEFDbContext>(sp => sp.GetRequiredService<TDbContext>());

        var options = new EfDbContextRegistrationOptions(typeof(TDbContext), services);

        optionsBuilder?.Invoke(options);
        services.TryAddTransient(RamshaDbContextOptionsFactory.Create<TDbContext>);

        foreach (var entry in options.ReplacedDbContextTypes)
        {
            var originalDbContextType = entry.Key;
            var targetDbContextType = entry.Value ?? typeof(TDbContext);

            services.Replace(ServiceDescriptor.Transient(originalDbContextType, sp =>
            {
                var dbContextType = sp.GetRequiredService<IEfDbContextTypeProvider>().GetDbContextType(originalDbContextType);
                return sp.GetRequiredService(dbContextType);
            }));

            services.Configure<RamshaDbContextOptions>(opts =>
            {
                opts.DbContextReplacements[originalDbContextType] = targetDbContextType;
            });
        }

        new EfRepositoryRegistrar(options).AddRepositories();
        new EFGlobalQueryFilterRegistrar(options).RegisterFilters();

        return services;
    }


}

public class RamshaDbContextOptionsExtension<TDbContext> : IDbContextOptionsExtension
where TDbContext : RamshaEFDbContext<TDbContext>
{
    public void ApplyServices(IServiceCollection services)
    {


    }

    public void Validate(IDbContextOptions options)
    {
    }

    public DbContextOptionsExtensionInfo Info => new RamshaOptionsExtensionInfo(this);

    private class RamshaOptionsExtensionInfo : DbContextOptionsExtensionInfo
    {
        public RamshaOptionsExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension)
        {
        }

        public override bool IsDatabaseProvider => false;

        public override int GetServiceProviderHashCode()
        {
            return 0;
        }

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
        {
            return other is RamshaOptionsExtensionInfo;
        }

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
        }

        public override string LogFragment => "RamshaOptionsExtension";
    }
}
