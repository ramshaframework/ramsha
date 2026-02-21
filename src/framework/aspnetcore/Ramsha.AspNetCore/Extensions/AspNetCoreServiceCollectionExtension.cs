
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Ramsha;
using Ramsha.AspNetCore;
using Ramsha.AspNetCore.Security.Claims;
using Ramsha.Files;
using Ramsha.Localization;
using Ramsha.Security.Claims;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class AspNetCoreServiceCollectionExtension
    {

        public static IServiceCollection AddRamshaAspNetCoreService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IPrincipalAccessor, HttpPrincipalAccessor>();
            services.TryAddSingleton<IRamshaAppPipeline<IApplicationBuilder>, RamshaAppPipeline<IApplicationBuilder>>();
            services.AddProblemDetails();
            services.AddExceptionHandler<RamshaGlobalExceptionHandler>();

            var pipelineContributorInterfaceType = typeof(IRamshaPipelineContributor);
            var contributors = RamshaTypeHelpers.GetRamshaTypes(pipelineContributorInterfaceType);
            foreach (var contributor in contributors)
            {
                services.AddSingleton(pipelineContributorInterfaceType, contributor);
            }

            services.AddTransient<IRamshaCookieService, RamshaCookieService>();

            services.AddSingleton<IRamshaRequestLocalizationOptionsProvider, RamshaRequestLocalizationOptionsProvider>();


            services.PostConfigure<RamshaFilesOptions>((options) =>
            {
                var env = services.GetRequiredService<IWebHostEnvironment>();
                options.PublicRootFilesPath ??= env.WebRootPath;
            });

            return services;
        }

        public static IWebHostEnvironment GetHostingEnvironment(this IServiceCollection services)
        {
            var hostingEnvironment = services.GetSingletonInstanceOrNull<IWebHostEnvironment>();

            if (hostingEnvironment == null)
            {
                return new EmptyHostingEnvironment()
                {
                    EnvironmentName = Environments.Development
                };
            }

            return hostingEnvironment;
        }
    }
}
