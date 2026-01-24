using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ramsha.EntityFrameworkCore;

public abstract class RamshaDesignTimeDbContext<TModule, TContext> : IDesignTimeDbContextFactory<TContext>
    where TModule : RamshaModule
    where TContext : DbContext
{
    public virtual TContext CreateDbContext(string[] args)
    {
        return AsyncHelper.RunSync(() => CreateDbContextAsync(args));
    }

    protected virtual async Task<TContext> CreateDbContextAsync(string[] args)
    {
        var application = await RamshaFactory.CreateAppAsync<TModule>(options =>
        {
            options.Services.ReplaceConfiguration(BuildConfiguration());
            ConfigureServices(options.Services);
        });

        await application.Initialize();

        return application.ServiceProvider.GetRequiredService<TContext>();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {

    }

    protected virtual IConfigurationRoot BuildConfiguration()
    {
        return ConfigurationHelper.BuildConfiguration();
    }
}
