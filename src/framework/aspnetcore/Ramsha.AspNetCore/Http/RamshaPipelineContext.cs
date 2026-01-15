using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ramsha.AspNetCore;

public sealed class RamshaPipelineContext
{
    private readonly IServiceProvider _services;

    public RamshaPipelineContext(IRamshaAppPipeline<IApplicationBuilder> pipeline, IServiceProvider services)
    {
        Pipeline = pipeline;
        _services = services;
    }

    public IRamshaAppPipeline<IApplicationBuilder> Pipeline { get; }

    public T GetRequiredService<T>()
    where T : notnull
    => _services.GetRequiredService<T>();

    public T? GetService<T>() => _services.GetService<T>();

    public IConfiguration GetConfiguration()
    {
        return GetRequiredService<IConfiguration>();
    }

    public IOptions<T> GetOptions<T>()
    where T : class
    {
        return GetRequiredService<IOptions<T>>();
    }
}
