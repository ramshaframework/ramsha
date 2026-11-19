
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Ramsha;
using Ramsha.AspNetCore;


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
                AsyncHelper.RunSync(() => ConfigureRamshaPipelineAsync(app));
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
                if (useRamshaPipeline)
                {
                    await ConfigureRamshaPipelineAsync(app);
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


        public static IApplicationBuilder UseRamshaRequestLocalization(this IApplicationBuilder app,
            Action<RequestLocalizationOptions>? optionsAction = null)
        {
            app.ApplicationServices
                .GetRequiredService<IRamshaRequestLocalizationOptionsProvider>()
                .InitializeOptions(optionsAction);

            return app.UseMiddleware<RamshaRequestLocalizationMiddleware>();
        }

        private static async Task ConfigureRamshaPipelineAsync(IApplicationBuilder app)
        {
            var pipe = app.ApplicationServices
            .GetService<IRamshaAppPipeline<IApplicationBuilder>>();

            if (pipe is null) return;

            await using var scope = app.ApplicationServices.CreateAsyncScope();
            var services = scope.ServiceProvider;
            var context = new RamshaPipelineContext(pipe, services);

            foreach (var cr in services.GetServices<IRamshaPipelineContributor>())
            {
                await cr.ConfigureAsync(context);
            }

            pipe.Apply(app);
        }

    }
}