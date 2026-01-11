using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Ramsha.Caching.Redis;

public class RedisCachingModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context.DependsOn<CachingModule>();
    }
    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);

        context.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = context.Configuration["Redis:Configuration"];
            options.InstanceName = context.Configuration["Redis:InstanceName"] ?? $"{Assembly.GetEntryAssembly()?.GetName().FullName}_";
        });
    }
}
