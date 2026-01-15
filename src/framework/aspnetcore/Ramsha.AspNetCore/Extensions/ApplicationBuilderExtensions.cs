
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Ramsha;
using Ramsha.AspNetCore;

namespace Ramsha.AspNetCore
{

}

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseRamsha(this IApplicationBuilder app, bool useRamshaPipeline = true)
        {
            var engine = app.ApplicationServices.GetRequiredService<IExternalRamshaEngine>();
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                AsyncHelper.RunSync(() => engine.ShutdownAsync());
            });

            if (engine is IDisposable disposableEngine)
            {
                applicationLifetime.ApplicationStopped.Register(disposableEngine.Dispose);
            }

            Task.Run(() => engine.Initialize(app.ApplicationServices))
            .GetAwaiter()
            .GetResult();


            if (useRamshaPipeline)
            {
                var pipeline = app.ApplicationServices.GetService<IRamshaAppPipeline<IApplicationBuilder>>();
                if (pipeline is not null)
                {
                    var contributors = app.ApplicationServices.GetServices<IRamshaPipelineContributor>();

                    foreach (var contributor in contributors)
                    {

                        using var scope = app.ApplicationServices.CreateScope();
                        var context = new RamshaPipelineContext(pipeline, scope.ServiceProvider);

                        contributor.ConfigureAsync(context)
                        .GetAwaiter()
                       .GetResult();
                    }
                    pipeline.Apply(app);
                }
            }
        }

        public static async Task UseRamshaAsync(
          this IApplicationBuilder app, bool useRamshaPipeline = true)
        {
            var engine = app.ApplicationServices.GetRequiredService<IExternalRamshaEngine>();
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                AsyncHelper.RunSync(() => engine.ShutdownAsync());
            });

            if (engine is IDisposable disposableEngine)
            {
                applicationLifetime.ApplicationStopped.Register(disposableEngine.Dispose);
            }

            await engine.Initialize(app.ApplicationServices);

            if (useRamshaPipeline)
            {
                var pipeline = app.ApplicationServices.GetService<IRamshaAppPipeline<IApplicationBuilder>>();
                if (pipeline is not null)
                {
                    var contributors = app.ApplicationServices.GetServices<IRamshaPipelineContributor>();

                    foreach (var contributor in contributors)
                    {
                        using var scope = app.ApplicationServices.CreateScope();
                        var context = new RamshaPipelineContext(pipeline, scope.ServiceProvider);
                        await contributor.ConfigureAsync(context);
                    }
                    pipeline.Apply(app);
                }
            }
        }


        public static IApplicationBuilder UseRamshaEndpoints(
       this IApplicationBuilder app,
       Action<IEndpointRouteBuilder>? additionalConfigurationAction = null)
        {
            var options = app.ApplicationServices
                .GetRequiredService<IOptions<RamshaEndpointRouterOptions>>()
                .Value;

            if (!options.EndpointConfigureActions.Any() && additionalConfigurationAction == null)
            {
                return app;
            }

            return app.UseEndpoints(endpoints =>
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    if (options.EndpointConfigureActions.Any())
                    {
                        var context = new EndpointRouteBuilderContext(endpoints, scope.ServiceProvider);

                        foreach (var configureAction in options.EndpointConfigureActions)
                        {
                            configureAction(context);
                        }
                    }
                }

                additionalConfigurationAction?.Invoke(endpoints);
            });
        }
    }
}