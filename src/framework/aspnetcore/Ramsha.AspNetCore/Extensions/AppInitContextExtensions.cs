using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ramsha.AspNetCore;

public static class AppInitContextExtensions
{
    public static IRamshaAppPipeline<IApplicationBuilder> GetAppPipelineBuilder(this InitContext context)
    {
        return context.ServiceProvider.GetRequiredService<IRamshaAppPipeline<IApplicationBuilder>>();
    }

    public static IWebHostEnvironment GetEnvironment(this InitContext context)
    {
        return context.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    }

    public static IConfiguration GetConfiguration(this InitContext context)
    {
        return context.ServiceProvider.GetRequiredService<IConfiguration>();
    }

    public static IOptions<T> GetOptions<T>(this InitContext context)
    where T : class
    {
        return context.ServiceProvider.GetRequiredService<IOptions<T>>();
    }

}
