using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;

namespace Ramsha.AspNetCore;

public class RamshaServiceProvidersFeatureFilter : IStartupFilter, IServiceProvidersFeature
{
    public RamshaServiceProvidersFeatureFilter(IServiceProvider serviceProvider)
    {
        RequestServices = serviceProvider;
    }

    public IServiceProvider RequestServices { get; set; }

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.Use(async (context, nxt) =>
            {
                context.Features.Set<IServiceProvidersFeature>(this);
                await nxt(context);
            });
            next(app);
        };
    }
}
